# Librotech Inspection


Librotech Inspection it is an application for analyzing 
the data collected by the [data logger](https://en.wikipedia.org/wiki/Data_logger).
The application is under active development.
(But at the moment I don’t have a windows computer at hand so development has stopped awhile)

At the moment, only russian language is available in the application.

### What is ready

The following functions are ready in the application - getting data from a .csv file,
building a chart, generating a short summary, displaying the configuration of the logger.

<p align="center">
    <img alt="Chart"
         src="img/chart-page.png"
         width="747" height="480">
    <img alt="Configuration"
         src="img/configuration-page.png"
         width="747" height="480">
</p>

### TODO

#### Important

 - Fix a bug - if there are two or more Y-axes on the chart, then only one Y-axis works correctly.
 - Fix major navigation performance issues.

#### The rest

 - Make settings (as a .json file)
 - Scope of `AppBootstrapper` is undefined, currently
it registers services, has a data loading method, navigation commands.
It needs to be rewritten and divided into several classes.
 - (Possibly unnecessary) Write a method to search for alarms.
 - Cover more code with unit tests.
