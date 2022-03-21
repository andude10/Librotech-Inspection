# Parsing chart data

The data that the user loads must contain the data registered by the logger. This is usually a table in the following
format:

| Date/Time           | Temperature | Humidity |
|---------------------|-------------|----------|
| 14:53:20 15.09.2021 | 23,5        | 45,3     |
| 14:53:20 15.09.2021 | 23,7        | 47,4     |
| ...                 | ...         | ...      |

Chart data parsers convert values from a table into the format required for charting.

For example List of `DataPoint` instances (a class used by the OxyPlot charting library) in which **X** - **Date/Time**
, **Y** -
**value** (such as temperature or humidity).