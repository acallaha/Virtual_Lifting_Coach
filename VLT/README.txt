The WPF application is run inside a MainWindow view. Each page of the interface corresponds to a *.xaml and *.xaml.cs page which runs inside this MainWindow view.

In addition to an .xaml and .xaml.cs file for each page, we have a classes for reps, sets, workouts, and sessions. After each session, this data is serialized to xml so the data persists between uses of the app. This allows users to view their lifting history in the “My Progress” page.
