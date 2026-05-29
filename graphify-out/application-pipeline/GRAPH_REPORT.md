# Graph Report - src/Application/Pipeline  (2026-05-29)

## Corpus Check
- Corpus is ~790 words - fits in a single context window. You may not need a graph.

## Summary
- 60 nodes · 68 edges · 7 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Fusion Cache Behavior|Fusion Cache Behavior]]
- [[_COMMUNITY_Cache Invalidation Behavior|Cache Invalidation Behavior]]
- [[_COMMUNITY_Performance Behavior|Performance Behavior]]
- [[_COMMUNITY_Result Exception Behavior|Result Exception Behavior]]
- [[_COMMUNITY_Validation Behavior|Validation Behavior]]
- [[_COMMUNITY_Logging Preprocessor|Logging Preprocessor]]
- [[_COMMUNITY_Request Counter|Request Counter]]

## God Nodes (most connected - your core abstractions)
1. `CacheInvalidationBehaviour` - 7 edges
2. `FusionCacheBehaviour` - 7 edges
3. `PerformanceBehaviour` - 7 edges
4. `ResultExceptionBehavior` - 6 edges
5. `ValidationBehavior` - 6 edges
6. `LoggingPreProcessor` - 4 edges
7. `RequestCounter` - 3 edges
8. `TRequest` - 2 edges
9. `TResponse` - 2 edges
10. `TRequest` - 2 edges

## Surprising Connections (you probably didn't know these)
- `CacheInvalidationBehaviour` --implements--> `IPipelineBehavior`  [EXTRACTED]
  CacheInvalidationBehaviour.cs →   _Bridges community 1 → community 0_
- `PerformanceBehaviour` --implements--> `IPipelineBehavior`  [EXTRACTED]
  PerformanceBehaviour.cs →   _Bridges community 0 → community 2_
- `ResultExceptionBehavior` --implements--> `IPipelineBehavior`  [EXTRACTED]
  ResultExceptionBehavior.cs →   _Bridges community 0 → community 3_
- `ValidationBehavior` --implements--> `IPipelineBehavior`  [EXTRACTED]
  ValidationBehavior.cs →   _Bridges community 0 → community 4_

## Communities (7 total, 0 thin omitted)

### Community 0 - "Fusion Cache Behavior"
Cohesion: 0.22
Nodes (9): CancellationToken, IAppCache, ILogger, MessageHandlerDelegate, TRequest, TResponse, ValueTask, IPipelineBehavior (+1 more)

### Community 1 - "Cache Invalidation Behavior"
Cohesion: 0.24
Nodes (8): CancellationToken, IAppCache, ILogger, MessageHandlerDelegate, TRequest, TResponse, ValueTask, CacheInvalidationBehaviour

### Community 2 - "Performance Behavior"
Cohesion: 0.28
Nodes (8): CancellationToken, ILogger, IUserContextAccessor, MessageHandlerDelegate, TRequest, TResponse, ValueTask, PerformanceBehaviour

### Community 3 - "Result Exception Behavior"
Cohesion: 0.28
Nodes (7): ResultExceptionBehavior, CancellationToken, ILogger, MessageHandlerDelegate, TRequest, TResponse, ValueTask

### Community 4 - "Validation Behavior"
Cohesion: 0.28
Nodes (7): IReadOnlyCollection, ValidationBehavior, CancellationToken, MessageHandlerDelegate, TRequest, TResponse, ValueTask

### Community 5 - "Logging Preprocessor"
Cohesion: 0.25
Nodes (6): CancellationToken, ILogger, IUserContextAccessor, TRequest, ValueTask, LoggingPreProcessor

### Community 6 - "Request Counter"
Cohesion: 0.50
Nodes (3): DateTime, int, RequestCounter

## Knowledge Gaps
- **30 isolated node(s):** `IAppCache`, `ILogger`, `MessageHandlerDelegate`, `CancellationToken`, `ValueTask` (+25 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PerformanceBehaviour` connect `Performance Behavior` to `Fusion Cache Behavior`, `Request Counter`?**
  _High betweenness centrality (0.300) - this node is a cross-community bridge._
- **Why does `CacheInvalidationBehaviour` connect `Cache Invalidation Behavior` to `Fusion Cache Behavior`?**
  _High betweenness centrality (0.233) - this node is a cross-community bridge._
- **What connects `IAppCache`, `ILogger`, `MessageHandlerDelegate` to the rest of the system?**
  _30 weakly-connected nodes found - possible documentation gaps or missing edges._