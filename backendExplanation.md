# RPSLS Game Backend - Onion Clean Architecture Implementation

## 📋 Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Domain Layer](#domain-layer)
3. [Application Layer](#application-layer)
4. [Infrastructure Layer](#infrastructure-layer)
5. [Presentation Layer](#presentation-layer)
6. [Dependency Flow](#dependency-flow)
7. [Key Design Patterns](#key-design-patterns)
8. [API Endpoints](#api-endpoints)

---

## 🏗️ Architecture Overview

The RPSLS Game backend follows **Onion Clean Architecture** principles with strict dependency inversion. The architecture consists of 4 main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                     │ ← External concerns
├─────────────────────────────────────────────────────────────┤
│                    Presentation Layer                      │ ← Controllers, DTOs
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                       │ ← Use Cases, Commands
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                          │ ← Business Logic (Core)
└─────────────────────────────────────────────────────────────┘
```

**Key Principle**: Dependencies flow inward only - outer layers depend on inner layers, never the reverse.

---

## 🎯 Domain Layer (Core Business Logic)
**Location**: `src/1.Domain/RpslsGameService.Domain/`
**Dependencies**: None (pure business logic)

### Enums

#### `ChoiceType.cs`
```csharp
public enum ChoiceType
{
    Rock = 1,
    Paper = 2,
    Scissors = 3,
    Lizard = 4,
    Spock = 5
}
```
- **Purpose**: Defines the 5 possible choices in the game
- **Design**: Uses explicit integer values for external API compatibility

#### `GameOutcome.cs`
```csharp
public enum GameOutcome
{
    Win,
    Lose,
    Tie
}
```
- **Purpose**: Represents the three possible game outcomes from player's perspective

### Value Objects

#### `Choice.cs`
- **Purpose**: Immutable value object representing a game choice
- **Key Features**:
  - Static factory methods for each choice (`Choice.Rock`, `Choice.Paper`, etc.)
  - Type-safe creation from ID or ChoiceType
  - Proper equality implementation
  - Contains both ID and name for external representation

```csharp
public static Choice FromId(int id) // Validates and creates choice from ID
public static Choice FromType(ChoiceType type) // Creates choice from enum
public static IReadOnlyList<Choice> GetAll() // Returns all available choices
```

#### `GameResult.cs`
- **Purpose**: Immutable value object containing complete game round result
- **Properties**:
  - `PlayerChoice`: Player's selected choice
  - `ComputerChoice`: Computer's generated choice
  - `Outcome`: Win/Lose/Tie from player perspective
  - `ResultMessage`: Human-readable description
  - `PlayedAt`: Timestamp of when the round was played

- **Smart Logic**: Generates contextual messages like "Rock crushes Scissors. You win!"

### Entities

#### `Entity.cs` (Base Class)
- **Purpose**: Abstract base class for all domain entities
- **Features**:
  - Unique `Guid Id` for each entity
  - Proper equality comparison based on ID and type
  - Overrides `Equals`, `GetHashCode`, and equality operators

#### `GameRound.cs`
- **Purpose**: Represents a single round of the game
- **Encapsulation**: Private constructor ensures controlled creation
- **Factory Method**: `Create()` method for proper instantiation
- **Conversion**: `ToGameResult()` converts to value object for external use

#### `GameSession.cs`
- **Purpose**: Aggregate root managing the entire game session
- **Encapsulated Collections**: Private `_rounds` list with read-only public access
- **Business Logic**:
  - `PlayRound()`: Executes a game round with business rule validation
  - `Reset()`: Clears game history
  - Statistics calculation (wins, losses, ties)
- **Domain Rules**: Contains the core RPSLS win condition logic

### Domain Services

#### `GameLogicService.cs`
- **Purpose**: Implements core game rules and win condition logic
- **Win Conditions Matrix**:
```csharp
Rock beats: Lizard, Scissors
Paper beats: Rock, Spock  
Scissors beats: Paper, Lizard
Lizard beats: Spock, Paper
Spock beats: Scissors, Rock
```
- **Pure Functions**: No side effects, only business logic

#### `ChoiceGenerationService.cs`
- **Purpose**: Converts random numbers to valid game choices
- **Algorithm**: `((randomNumber - 1) % 5) + 1` ensures valid choice (1-5)

### Exceptions

#### `DomainException.cs` (Base)
- **Purpose**: Base class for all domain-specific exceptions
- **Inheritance**: Extends `Exception` with domain context

#### `InvalidChoiceException.cs`
- **Purpose**: Thrown when invalid choice IDs are provided
- **Context**: Includes the invalid ID for debugging

#### `GameLogicException.cs`
- **Purpose**: Thrown when game logic violations occur

### Interfaces

#### `IGameLogicService.cs`
```csharp
GameResult DetermineWinner(Choice playerChoice, Choice computerChoice);
IReadOnlyList<Choice> GetAllChoices();
```

#### `IChoiceGenerationService.cs`
```csharp
Choice GenerateComputerChoice(int randomNumber);
```

---

## 🔄 Application Layer (Use Cases & CQRS)
**Location**: `src/2.Application/RpslsGameService.Application/`
**Dependencies**: Domain Layer only

### CQRS Commands

#### `PlayGameCommand.cs`
```csharp
public record PlayGameCommand(int PlayerChoice) : IRequest<GameResultResponse>;
```
- **Purpose**: Command to execute a game round
- **Immutable**: Uses C# record for immutability

#### `PlayGameCommandHandler.cs`
- **Orchestration**: Coordinates domain services and external dependencies
- **Flow**:
  1. Validate player choice
  2. Get random number from external service
  3. Generate computer choice
  4. Execute game logic
  5. Save game state
  6. Return mapped result

#### `ResetScoreboardCommand.cs` & Handler
- **Purpose**: Clears game history and statistics
- **Simple**: No complex orchestration needed

### CQRS Queries

#### `GetChoicesQuery.cs` & Handler
- **Purpose**: Returns all available game choices
- **Caching Friendly**: Static data that can be cached

#### `GetRandomChoiceQuery.cs` & Handler
- **Purpose**: Returns a random computer choice
- **External Dependency**: Uses random number service

#### `GetGameHistoryQuery.cs` & Handler
- **Purpose**: Returns game history with statistics
- **Aggregation**: Combines rounds with calculated statistics

### DTOs (Data Transfer Objects)

#### `ChoiceDto.cs`
```csharp
public record ChoiceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
```

#### `PlayGameRequest.cs`
```csharp
public record PlayGameRequest
{
    public int Player { get; init; }
}
```

#### `GameResultResponse.cs`
- **Complete Result**: Contains all game round information
- **API Friendly**: Formatted for JSON serialization

#### `GameHistoryResponse.cs`
- **Aggregated Data**: History + statistics in one response

### Validators (FluentValidation)

#### `PlayGameRequestValidator.cs`
```csharp
RuleFor(x => x.Player)
    .InclusiveBetween(1, 5)
    .WithMessage("Player choice must be between 1 and 5");
```

#### `PlayGameCommandValidator.cs`
- **Command Validation**: Validates at the command level
- **Consistent Rules**: Same validation as DTO

### AutoMapper Mappings

#### `GameMappingProfile.cs`
- **Domain to DTO**: Maps domain objects to API responses
- **Outcome Mapping**: Converts enum to string for API
- **Custom Logic**: Handles complex property mappings

### Cross-Cutting Behaviors

#### `ValidationBehavior.cs`
- **MediatR Pipeline**: Validates commands before execution
- **Automatic**: Uses registered FluentValidation validators
- **Exception Handling**: Throws `ValidationException` on failure

### Interfaces

#### `IRandomNumberService.cs`
```csharp
Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default);
```

#### `IGameSessionRepository.cs`
- **Repository Pattern**: Abstracts data persistence
- **Async**: All methods return `Task` for async operations
- **Session Management**: Handles current session concept

---

## 🔌 Infrastructure Layer (External Concerns)
**Location**: `src/3.Infrastructure/RpslsGameService.Infrastructure/`
**Dependencies**: Application and Domain layers

### External Services

#### `HttpRandomNumberService.cs`
- **Purpose**: Implements `IRandomNumberService` using HTTP client
- **Resilience**: Polly retry policies and circuit breakers
- **Fallback**: Local random number generator when service fails
- **Logging**: Comprehensive logging for debugging
- **Configuration**: Configurable timeouts and retry counts

**Resilience Patterns**:
- **Retry Policy**: Exponential backoff with configurable attempts
- **Circuit Breaker**: Opens after 3 consecutive failures
- **Fallback**: Uses `System.Random` when external service unavailable

### Persistence

#### `InMemoryGameSessionRepository.cs`
- **Implementation**: In-memory storage for game sessions
- **Thread Safety**: Uses locking for concurrent access
- **Session Management**: Maintains current session concept
- **Lazy Creation**: Creates session on first access

### Caching

#### `ICacheService.cs`
- **Abstraction**: Generic caching interface
- **Type Safety**: Generic methods with proper constraints
- **Async**: All operations are asynchronous

#### `InMemoryCacheService.cs`
- **Implementation**: Uses `IMemoryCache` from .NET
- **Configuration**: Configurable expiration times
- **Error Handling**: Graceful failure - returns null on errors
- **Eviction**: Sliding expiration and priority-based eviction

### Configuration

#### `ExternalApiSettings.cs`
```csharp
public class ExternalApiSettings
{
    public const string SectionName = "ExternalApis";
    public RandomNumberServiceSettings RandomNumberService { get; set; }
}
```

#### `CachingSettings.cs`
- **Strongly Typed**: Configuration bound to classes
- **Defaults**: Sensible default values provided

### Dependency Injection

#### `ServiceCollectionExtensions.cs`
- **Modular Registration**: Organized by concern
- **HTTP Client**: Configured with base URL and timeout
- **Polly Integration**: Retry and circuit breaker policies
- **Logging**: Serilog configuration

**Key Registrations**:
```csharp
services.AddHttpClient<IRandomNumberService, HttpRandomNumberService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());
```

---

## 🌐 Presentation Layer (Web API)
**Location**: `src/4.Presentation/RpslsGameService.Api/`
**Dependencies**: Application layer only

### Controllers

#### `GameController.cs`
- **RESTful Design**: Follows REST conventions
- **MediatR Integration**: All actions use MediatR commands/queries
- **Documentation**: Comprehensive XML documentation
- **Error Handling**: Relies on middleware for exception handling

**Endpoints**:
- `GET /api/game/choices` - Get all choices
- `GET /api/game/choice` - Get random choice
- `POST /api/game/play` - Play game round
- `GET /api/game/history` - Get game history
- `DELETE /api/game/reset` - Reset game

#### `HealthController.cs`
- **Simple Health Check**: Returns service status
- **Monitoring**: Used by load balancers and monitoring tools

### Middleware

#### `ExceptionHandlingMiddleware.cs`
- **Centralized**: All exceptions handled in one place
- **HTTP Status Mapping**: Maps exception types to appropriate HTTP codes
- **Logging**: Logs all exceptions for debugging
- **User Friendly**: Returns structured error responses

**Exception Mapping**:
```csharp
ValidationException → 400 Bad Request
DomainException → 400 Bad Request  
ArgumentException → 400 Bad Request
Unknown → 500 Internal Server Error
```

### Configuration

#### `ServiceCollectionExtensions.cs`
- **Service Registration**: Organizes all service registrations
- **Swagger**: Configured with XML documentation
- **CORS**: Allows all origins (development only)
- **MediatR**: Registers all handlers from Application layer

#### `Program.cs`
- **Application Bootstrap**: Configures and starts the application
- **Middleware Pipeline**: Proper ordering of middleware
- **Serilog**: Request logging configuration
- **Development vs Production**: Different configurations

### Configuration Files

#### `appsettings.json`
```json
{
  "ExternalApis": {
    "RandomNumberService": {
      "BaseUrl": "https://codechallenge.boohma.com",
      "TimeoutSeconds": 30,
      "RetryCount": 3,
      "EnableFallback": true
    }
  },
  "Caching": {
    "DefaultExpiration": "00:05:00"
  }
}
```

#### `appsettings.Development.json`
- **Development Overrides**: Faster timeouts, more logging
- **Debug Configuration**: Enhanced logging levels

---

## 🔄 Dependency Flow

### Dependency Inversion Principle
```
Presentation → Application → Domain ← Infrastructure
```

**Key Points**:
- Domain has NO dependencies (pure business logic)
- Application depends only on Domain abstractions
- Infrastructure implements Application interfaces
- Presentation orchestrates via Application layer

### Dependency Injection Flow
1. **Domain Services**: Registered in Presentation layer
2. **Application Services**: MediatR, AutoMapper, FluentValidation
3. **Infrastructure Services**: External APIs, repositories, caching
4. **Presentation Services**: Controllers, middleware, Swagger

---

## 🎨 Key Design Patterns

### 1. **Domain-Driven Design (DDD)**
- **Entities**: `GameSession`, `GameRound`
- **Value Objects**: `Choice`, `GameResult`
- **Domain Services**: `GameLogicService`
- **Aggregate Roots**: `GameSession` manages `GameRound`s

### 2. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Modify state (`PlayGameCommand`, `ResetScoreboardCommand`)
- **Queries**: Read data (`GetChoicesQuery`, `GetGameHistoryQuery`)
- **Handlers**: Separate logic for commands and queries

### 3. **Repository Pattern**
- **Abstraction**: `IGameSessionRepository` in Application layer
- **Implementation**: `InMemoryGameSessionRepository` in Infrastructure
- **Testability**: Easy to mock for unit tests

### 4. **Factory Pattern**
- **Static Factories**: `Choice.Rock`, `Choice.FromId()`
- **Factory Methods**: `GameSession.Create()`, `GameRound.Create()`

### 5. **Strategy Pattern**
- **Game Logic**: Win conditions implemented as dictionary lookup
- **Choice Generation**: Abstracted via `IChoiceGenerationService`

### 6. **Decorator Pattern**
- **Validation Behavior**: Decorates MediatR pipeline
- **Logging**: Serilog decorates built-in logging

### 7. **Circuit Breaker & Retry Patterns**
- **Polly Integration**: Resilience for external API calls
- **Graceful Degradation**: Fallback to local random generation

---

## 🚀 API Endpoints

### Game Operations

#### `GET /api/game/choices`
**Response**: List of all available choices
```json
[
  { "id": 1, "name": "Rock" },
  { "id": 2, "name": "Paper" },
  { "id": 3, "name": "Scissors" },
  { "id": 4, "name": "Lizard" },
  { "id": 5, "name": "Spock" }
]
```

#### `GET /api/game/choice`
**Response**: Random computer choice
```json
{ "id": 3, "name": "Scissors" }
```

#### `POST /api/game/play`
**Request**:
```json
{ "player": 1 }
```
**Response**:
```json
{
  "results": "win",
  "player": 1,
  "computer": 3,
  "playerChoice": "Rock",
  "computerChoice": "Scissors", 
  "message": "Rock crushes Scissors. You win!",
  "playedAt": "2024-01-15T10:30:00Z"
}
```

#### `GET /api/game/history`
**Response**: Game history with statistics
```json
{
  "history": [/* array of GameResultResponse */],
  "totalGames": 10,
  "playerWins": 4,
  "computerWins": 3,
  "ties": 3
}
```

#### `DELETE /api/game/reset`
**Response**: `204 No Content`

### Health Check

#### `GET /health`
**Response**:
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## 🏆 Architecture Benefits

### 1. **Testability**
- **Pure Domain**: Easy to unit test without external dependencies
- **Dependency Injection**: Easy to mock dependencies
- **CQRS**: Test commands and queries separately

### 2. **Maintainability**
- **Clear Boundaries**: Each layer has specific responsibilities
- **Low Coupling**: Layers communicate via abstractions
- **High Cohesion**: Related functionality grouped together

### 3. **Scalability**
- **Stateless**: API is stateless (except for in-memory storage)
- **Async**: All I/O operations are asynchronous
- **Caching**: Built-in caching support

### 4. **Reliability**
- **Resilience**: Circuit breaker and retry patterns
- **Error Handling**: Comprehensive exception handling
- **Logging**: Structured logging throughout

### 5. **Flexibility**
- **Swappable Infrastructure**: Easy to change external services
- **Configuration**: Externalized configuration
- **Extensibility**: New features can be added without breaking existing code

---

## 🧪 Testing Strategy

### Unit Tests (Recommended)
- **Domain Layer**: Test business logic in isolation
- **Application Layer**: Test command/query handlers with mocks
- **Infrastructure Layer**: Test external service integrations

### Integration Tests (Recommended)
- **API Tests**: Test controllers end-to-end
- **Repository Tests**: Test data persistence
- **External Service Tests**: Test with real or stubbed services

### Architecture Tests (Recommended)
- **Dependency Rules**: Ensure layers don't violate dependency rules
- **Naming Conventions**: Ensure consistent naming
- **Attribute Usage**: Ensure proper attribute usage

---

This backend implementation provides a solid foundation for the RPSLS game with enterprise-grade patterns, comprehensive error handling, and excellent separation of concerns through Onion Clean Architecture.