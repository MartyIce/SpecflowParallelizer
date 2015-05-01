# Specflow Parallelizer

## Overview
This solution starts with a specflow assembly, creates a nant build file (using .Net) that uses nunit to run a unit tests in parallel.  This *possibly* will give you a better Specflow experience, provided:

* there are no concurrency violation possibilities between tests.  eg, you can't have two specflow tests that trample on the same database records, things like that.
* the better you distribute your test Fixtures across namespaces, the better this will (hopefully) work.  The nant script will spawn a thread per namespace (with thread count throttled by config), so namespaces with a large number of tests will take a longer time to run.
* your tests don't break something in the specflow engine.  It's not quite clear why specflow doesn't just run in parallel (but it looks like they charge money for a tool that enables it), but it seems to have something to do with ScenarioContext.Current being a singleton?  Here's some info I found on that:
  * http://elegantcode.com/2013/08/30/some-new-features-for-specflow-and-specrun/
  * http://elegantcode.com/2013/08/30/some-new-features-for-specflow-and-specrun/

I started playing around with this idea, and while it didn't (hasn't) worked out yet for my project, I was hoping people might get some use out of it (and improve it).  For the tests I'm trying to parallelize, it completely pegs my 8 cores (whereas just running the assembly as a whole, serially, really doesn't tax my CPU much at all).  It doesn't work for me because I get a ton of concurrency violations from Elasticsearch (the underlying tests weren't written with concurrency in mind, and trample data).  Hopefully you have a better experience...

## Config

Executing the C# console application contained in the BuildFileGenerator folder will generate a nant build file.  The app.config needs to be tweaked with the following values:

    <add key="testAssemblyLocation" value="C:\Temp\MyTestAssembly.dll"/>  <!-- location of your test assembly -->
    <add key="buildFileLocation" value="C:\Temp\parallel.tests.build"/>  <!-- desired location of your outputted nant build file -->
    <add key="testOutputDirectory" value="C:\Temp\TestResults"/>  <!-- this will be used as part of the nant file generation, its where your tests will be configured to output their results -->

The resulting nant file will contain a list of tasks for each namespace in the test assembly above, contained within a custom "parallelexec" task.  The "parallelexec" definition is included inline in the build script.  The  "example.build" file is included in this repo, it shows what a geneated file might look like

Once you get the nant file generated (and nant/nunit properly installed), one other thing to take note of is the generated file can be overridden to tweak the max concurrent # of threads.  Here's what that code looks like, you should see it in your generated output:

```
	parallelOptions.MaxDegreeOfParallelism = 6;
```

Also, there's some example error handling here that's commented out, tweak to your heart's delight.

Once the build file is ready, simply run "nant".  This will launch your specflow tests in parallel, and hopefully bring you joy.