### Overview
This is the solution for Sievo back end assignment, done by Phuc Tran

### Run instruction
The solution should run out of the box with a fresh dotnet install. In details, you'll need:
- netcoreapp3.1 runtime
- C# 8.0 (which is supported in any targets older than netcoreapp3.0)

Some quick commands to try with dotnet cli, given you are at solution's root:
- First create some executable with ```dotnet publish```
- Then you can try it out with the test file:
```
./SievoAssignment/bin/Debug/netcoreapp3.1/publish/SievoAssignment.exe --file ./SievoAssignment.Tests.Unit/ExampleData.tsv --project 3
```
```
./SievoAssignment/bin/Debug/netcoreapp3.1/publish/SievoAssignment.exe --file ./SievoAssignment.Tests.Unit/ExampleData.tsv --sortByStartDate 
```

### Test
```dotnet test``` shall do

### Disclaimer
This repo is not affiliated with Sievo the company. The repo will be made private shortly after it has done its job :)

### Contact
phuc.trandt (at) outlook.com

