# Visual Studio Solution Fix

## Problem Identified
The solution file and project references have incorrect paths causing Visual Studio to look for projects outside the `src` folder.

## Current Directory Structure
```
RPSLS.Service/
├── RpslsGameService.sln
└── src/
    ├── 1.Domain/
    │   └── RpslsGameService.Domain/
    │       └── RpslsGameService.Domain.csproj
    ├── 2.Application/
    │   └── RpslsGameService.Application/
    │       └── RpslsGameService.Application.csproj
    ├── 3.Infrastructure/
    │   └── RpslsGameService.Infrastructure/
    │       └── RpslsGameService.Infrastructure.csproj
    └── 4.Presentation/
        └── RpslsGameService.Api/
            └── RpslsGameService.Api.csproj
```

## Fix #1: Solution File (RpslsGameService.sln)
Replace the entire content with:
```xml
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{E3C0A4D7-3C32-4E1D-8C3F-E3F2B2F67D20}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "RpslsGameService.Domain", "src\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj", "{12345678-1234-1234-1234-123456789001}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "RpslsGameService.Application", "src\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj", "{12345678-1234-1234-1234-123456789002}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "RpslsGameService.Infrastructure", "src\3.Infrastructure\RpslsGameService.Infrastructure\RpslsGameService.Infrastructure.csproj", "{12345678-1234-1234-1234-123456789003}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "RpslsGameService.Api", "src\4.Presentation\RpslsGameService.Api\RpslsGameService.Api.csproj", "{12345678-1234-1234-1234-123456789004}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{12345678-1234-1234-1234-123456789001}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{12345678-1234-1234-1234-123456789001}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{12345678-1234-1234-1234-123456789001}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{12345678-1234-1234-1234-123456789001}.Release|Any CPU.Build.0 = Release|Any CPU
		{12345678-1234-1234-1234-123456789002}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{12345678-1234-1234-1234-123456789002}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{12345678-1234-1234-1234-123456789002}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{12345678-1234-1234-1234-123456789002}.Release|Any CPU.Build.0 = Release|Any CPU
		{12345678-1234-1234-1234-123456789003}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{12345678-1234-1234-1234-123456789003}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{12345678-1234-1234-1234-123456789003}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{12345678-1234-1234-1234-123456789003}.Release|Any CPU.Build.0 = Release|Any CPU
		{12345678-1234-1234-1234-123456789004}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{12345678-1234-1234-1234-123456789004}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{12345678-1234-1234-1234-123456789004}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{12345678-1234-1234-1234-123456789004}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{12345678-1234-1234-1234-123456789001} = {E3C0A4D7-3C32-4E1D-8C3F-E3F2B2F67D20}
		{12345678-1234-1234-1234-123456789002} = {E3C0A4D7-3C32-4E1D-8C3F-E3F2B2F67D20}
		{12345678-1234-1234-1234-123456789003} = {E3C0A4D7-3C32-4E1D-8C3F-E3F2B2F67D20}
		{12345678-1234-1234-1234-123456789004} = {E3C0A4D7-3C32-4E1D-8C3F-E3F2B2F67D20}
	EndGlobalSection
EndGlobal
```

## Fix #2: Application Project (src/2.Application/RpslsGameService.Application/RpslsGameService.Application.csproj)
Change line 18 from:
```xml
<ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```
To:
```xml
<ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

## Fix #3: Infrastructure Project (src/3.Infrastructure/RpslsGameService.Infrastructure/RpslsGameService.Infrastructure.csproj)
Change the ProjectReference section from:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
  <ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
</ItemGroup>
```
To:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
  <ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
</ItemGroup>
```

## Fix #4: API Project (src/4.Presentation/RpslsGameService.Api/RpslsGameService.Api.csproj)
Change the ProjectReference section from:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
  <ProjectReference Include="..\..\..\3.Infrastructure\RpslsGameService.Infrastructure\RpslsGameService.Infrastructure.csproj" />
  <ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
</ItemGroup>
```
To:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
  <ProjectReference Include="..\..\3.Infrastructure\RpslsGameService.Infrastructure\RpslsGameService.Infrastructure.csproj" />
  <ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
</ItemGroup>
```

## Summary
- The solution file needs to be simplified to just include the 4 main projects
- All project references need to have one `../` removed (change `../../../` to `../../`)
- This is because from a project file, you go up 2 levels to reach the `src` folder, not 3

## After Fixing
1. Save all files
2. Open in Visual Studio
3. Build → Rebuild Solution
4. All projects should load and build successfully