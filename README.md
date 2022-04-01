# Librotech Inspection


Librotech Inspection it is an application for analyzing 
the data collected by the data logger.
The application is under active development.

At the moment, only russian language is available in the application.

![Chart](https://github.com/andude10/Librotech-Inspection/tree/main/img/chart-page.png?raw=true "Chart")

![Logger configuration](https://github.com/andude10/Librotech-Inspection/tree/main/img/configuration-page.png?raw=true "Logger configuration")


### What is ready

The application is currently able to build a chart (which can be zoomed), 
display a short summary of the data, display alarm settings.
In ConfigurationView you can see the configuration of the logger.

### TODO

#### Important

 - If there are more than two series on the chart, then only one Y axis works correctly
 - Serious performance issues when navigating
 - Highlight selected page button

#### The rest

 - Make a settings (as .json)
 - The application area  of `AppBootstrapper` is not defined, and 
now it registers services, has data loading method and navigation commands. 
It needs to be rewritten and divided into several classes.
 - Add russian documentation
 - Write a method to search for alarms in the data
 - Cover more code with unit tests

