using Build;

var testProject = args.Skip(0).FirstOrDefault() ?? "TestProduct";
await Builder.Run(testProject);
