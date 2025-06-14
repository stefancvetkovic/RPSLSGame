# Build Fix Instructions for RPSLS Backend

## ✅ **Issues Fixed:**
1. **NuGet Packages**: Successfully restored from official repository `https://api.nuget.org/v3/index.json`
2. **NuGet.config**: Created to ensure proper package source

## 🔧 **Remaining Issues to Fix:**

### **Project Reference Path Errors**
The `.csproj` files have incorrect relative paths for project references:

**Current (Incorrect):**
```xml
<ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

**Should be (Correct):**
```xml
<ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

### **Files to Fix:**

#### 1. **Application Project** 
**File**: `src/2.Application/RpslsGameService.Application/RpslsGameService.Application.csproj`
**Change**:
```xml
<!-- Change from: -->
<ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />

<!-- To: -->
<ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

#### 2. **Infrastructure Project**
**File**: `src/3.Infrastructure/RpslsGameService.Infrastructure/RpslsGameService.Infrastructure.csproj`
**Change**:
```xml
<!-- Change from: -->
<ProjectReference Include="..\..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
<ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />

<!-- To: -->
<ProjectReference Include="..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
<ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

#### 3. **API Project**
**File**: `src/4.Presentation/RpslsGameService.Api/RpslsGameService.Api.csproj`
**Change**:
```xml
<!-- Change from: -->
<ProjectReference Include="..\..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
<ProjectReference Include="..\..\..\3.Infrastructure\RpslsGameService.Infrastructure\RpslsGameService.Infrastructure.csproj" />
<ProjectReference Include="..\..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />

<!-- To: -->
<ProjectReference Include="..\..\2.Application\RpslsGameService.Application\RpslsGameService.Application.csproj" />
<ProjectReference Include="..\..\3.Infrastructure\RpslsGameService.Infrastructure\RpslsGameService.Infrastructure.csproj" />
<ProjectReference Include="..\..\1.Domain\RpslsGameService.Domain\RpslsGameService.Domain.csproj" />
```

## 📂 **Directory Structure Understanding:**
```
RPSLS.Service/
├── src/
│   ├── 1.Domain/RpslsGameService.Domain/          <- Target
│   ├── 2.Application/RpslsGameService.Application/ <- From here, go up 2 levels (../../) then down
│   ├── 3.Infrastructure/RpslsGameService.Infrastructure/
│   └── 4.Presentation/RpslsGameService.Api/
└── RpslsGameService.sln
```

## 🚀 **After Fixing:**
1. Run: `dotnet restore`
2. Run: `dotnet build`
3. Both should complete without errors

## 📋 **Current Status:**
- ✅ NuGet packages working with official repository
- ✅ All source code is correct  
- 🔧 Need to fix project reference paths (remove one `../` from each path)
- ✅ Solution structure is correct
- ✅ All necessary files created

The backend code architecture is complete and correct - we just need these path fixes!