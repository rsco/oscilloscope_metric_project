# oscilloscope_metric_project
A Winforms app that produces some metrics through data collected from UTFPR Oscilloscope.

To run the app you need to change:

1. The FILEPATH in line 36: 
   private const string FILEPATH = @"C:\Users\oliveir\Desktop\Temp\{0}"; (Put the filepath of your machine)
   
2. At line 248 you can pass true if you would like to check the metric of WRITE operation or false for READ:
   Application.Run(new OscilloscopeMetricsChart(true));
   
3. At line 142 you can choose the file you would like to extract by the index of the array created, for instance:
   string fileName = filenameArr[0]; //LINUX0

