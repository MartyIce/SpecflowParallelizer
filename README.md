# SpecflowParallelizer

## Overview
This solution starts with a specflow assembly, creates a nant build file (using .Net) that uses nunit to run a unit tests in parallel.  This *possibly* will give you a better Specflow experience, provided:

* no concurrency violation possibilities between tests.  For example, you can't have two specflow tests that trample on the same database records, etc.
* the better you distribute your test Fixtures across namespaces, the better this will probably work.  The logic spawns a thread per namespace (with thread count throttled by config), so Namespaces with a large number of tests will take a longer time to run.

I started playing around with this idea, and while it didn't (hasn't) worked out yet for my project, I was hoping people might get some use out of it (and hopefully improve it).  For the tests I'm trying to parallelize, it completely pegs 8 cores (whereas just running the assembly as a whole, serially, really doesn't tax my CPU).  However, I get a ton of concurrency violations in my web services (they're legacy tests, and have "trampling" data).  Hopefully you have a better experience...

## Config

Executing the console application in the BuildFileGenerator folder will generate a nant build file.  The app.config needs to be tweaked with the following values:

    <add key="testAssemblyLocation" value="C:\Temp\MyTestAssembly.dll"/>  <!-- location of your test assembly -->
    <add key="buildFileLocation" value="C:\Temp\parallel.tests.build"/>  <!-- desired location of your outputted nant build file -->
    <add key="testOutputDirectory" value="C:\Temp\TestResults"/>  <!-- this will be used as part of the nant file generation, its where your tests will be configured to output their results -->

The resulting nant file will contain a list of tasks for each namespace in the test assembly above, contained within a custom "parallel exec" task.  "example.build" file is included here, this is what the nant file should look like after generation. 

Once you get the nant file generated (and nant/nunit properly installed), one other thing to take note of is the generated file can be overridden to tweak the max concurrent # of threads.  Here's what that code looks like:

```
	parallelOptions.MaxDegreeOfParallelism = 6;
```

Also, there's some example error handling here that's commented out, tweak to your heart's delight.

Once the build file is ready,  simply run "nant".  This will launch your specflow tests in parallel!

I'd love to hear from people who have good results with this tool, I was going to just forget about it, but figured I'd toss it up here and see if someone can get some benefit from it.  Thanks!