# Librotech Inspection


Librotech Inspection it is an application for analyzing 
the data collected by the data logger.
The application is under active development.

At the moment, only russian language is available in the application.

### What is ready

The application is currently able to build a chart (which can be zoomed), 
display a short summary of the data, display alarm settings.
In ConfigurationView you can see the configuration of the logger.

### TODO

#### Important

 - If there are more than two series on the chart, then only one Y axis works correctly
 - Serious performance issues when navigating

#### The rest

 - Write a method to search for alarms in the data
 - ~~Design the `LoggerConfigurationView.xaml page.` The idea is to make a preview of the data (for example, the configuration), to view all the data you need to click on the `Show all` button.~~
 - Cover more code with unit tests

