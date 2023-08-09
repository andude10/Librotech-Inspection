using System;
using System.Collections.Generic;
using System.Linq;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Desktop.Utilities.Interactions;

namespace LibrotechInspection.Desktop.Services;

public class ParserValidatorRule
{
    private readonly List<bool> _conditions = new();

    public ParserValidatorRule(bool firstCondition)
    {
        _conditions.Add(firstCondition);
    }

    public bool IsTrue => _conditions.Any(rule => rule);

    public void AddCondition(bool condition)
    {
        _conditions.Add(condition);
    }
}

public class ParserValidatorAction
{
    private readonly ParserValidator _validator;

    public ParserValidatorAction(ParserValidator validator)
    {
        _validator = validator;
    }

    public ParserValidatorAction Or(Predicate<ParserResult> predicate)
    {
        _validator.Rules.Last().AddCondition(predicate(_validator.ParserResult));
        return this;
    }


    public ParserValidatorAction And(Predicate<ParserResult> predicate)
    {
        _validator.Rules.Add(new ParserValidatorRule(predicate(_validator.ParserResult)));
        return this;
    }

    public ParserValidator ShowInternalError(string message)
    {
        if (_validator.Rules.Any(rule => !rule.IsTrue)) return _validator;
        Interactions.Error.InnerException.Handle(message).Subscribe();
        return _validator;
    }


    public ParserValidator ShowExternalError(string message)
    {
        if (_validator.Rules.Any(rule => !rule.IsTrue)) return _validator;
        Interactions.Error.ExternalError.Handle(message).Subscribe();
        return _validator;
    }

    public ParserValidator Warn(string message)
    {
        if (_validator.Rules.Any(rule => !rule.IsTrue)) return _validator;
        Interactions.Error.ExternalError.Handle(message).Subscribe();
        return _validator;
    }
}

// TODO: Write tests for ParserValidator
public class ParserValidator
{
    public readonly List<ParserValidatorRule> Rules = new();

    public ParserValidator(ParserResult parserResult)
    {
        ParserResult = parserResult;
    }

    public ParserResult ParserResult { get; }

    public ParserValidator IfSuccessful()
    {
        Rules.Add(new ParserValidatorRule(ParserResult.FileRecord is not null & ParserResult.ParserException is null));
        return this;
    }

    public ParserValidator IfNotSuccessful()
    {
        Rules.Add(new ParserValidatorRule(ParserResult.FileRecord is null | ParserResult.ParserException is not null));
        return this;
    }

    public ParserValidatorAction When(Predicate<ParserResult> predicate)
    {
        Rules.Add(new ParserValidatorRule(predicate(ParserResult)));
        return new ParserValidatorAction(this);
    }

    public ParserValidatorAction OrWhen(Predicate<ParserResult> predicate)
    {
        if (Rules.Count < 2) return When(predicate);
        Rules.RemoveAt(Rules.Count - 1);
        Rules.Add(new ParserValidatorRule(predicate(ParserResult)));
        return new ParserValidatorAction(this);
    }
}