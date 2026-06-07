## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

When the user types `/graphify`, invoke the `skill` tool with `skill: "graphify"` before doing anything else.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- Dirty graphify-out/ files are expected after hooks or incremental updates; dirty graph files are not a reason to skip graphify. Only skip graphify if the task is about stale or incorrect graph output, or the user explicitly says not to use it.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## Purpose

This file helps AI coding agents work effectively in this repository. Use it as a fast orientation guide before making changes.

## Project Overview

This repository is a Clean Architecture Blazor Server solution template built for long-term maintainability, modular feature development, and enterprise-style application patterns.

The main application layers are:

- `Domain`: core entities, domain events, and business rules
- `Application`: use cases, DTOs, validation, pipeline behaviors, and application abstractions
- `Infrastructure`: persistence, external integrations, identity, caching, and runtime services
- `Server.UI`: Blazor Server UI, pages, components, and client-facing application wiring
- `Migrators`: EF Core migration entry points and database update tooling

## Solution Map

- `src/Application`
  Application logic and feature implementation. Start here for commands, queries, validators, specifications, and service contracts.
- `src/Domain`
  Domain model and business concepts. Keep this layer independent of UI and infrastructure concerns.
- `src/Infrastructure`
  EF Core, authentication, file services, caching, background jobs, and other external dependencies.
- `src/Migrators`
  Database migration projects used for provider-specific EF Core migration work.
- `src/Server.UI`
  Blazor Server host, pages, components, menus, dialogs, and UI services.
- `tests/Application.UnitTests`
  Unit tests for application-layer behavior.
- `tests/Application.IntegrationTests`
  Integration-style tests for application workflows and persistence-backed behavior.
- `tests/Domain.UnitTests`
  Unit tests focused on domain behavior.
- `tests/Infrastructure.UnitTests`
  Tests for infrastructure services and related helpers.
- `docs/`
  Supporting project documentation.
- `docs/superpowers/`
  Recommended place for design notes, plans, and implementation specs when a task needs explicit planning.

## Working Guidance

- Preserve Clean Architecture boundaries. Avoid introducing UI or infrastructure concerns into `Domain`.
- Prefer existing repository patterns over inventing new ones. Match nearby features before creating new abstractions.
- When adding a new feature, inspect similar modules in `Application`, `Infrastructure`, and `Server.UI` first.
- Keep changes targeted. Do not refactor unrelated areas unless the task requires it.
- Update documentation when setup steps, commands, or workflows change.
- If a task benefits from explicit design or planning, add artifacts under `docs/superpowers/` rather than inventing a separate process.

## Common Commands

Build the solution:

```bash
dotnet build CleanArchitecture.Blazor.slnx
```

Run tests:

```bash
dotnet test CleanArchitecture.Blazor.slnx
```

Run the Blazor Server app:

```bash
dotnet run --project src/Server.UI
```

Add an EF Core migration for SQL Server:

```bash
dotnet ef migrations add InitialCreate --project src/Migrators/Migrators.MSSQL --startup-project src/Server.UI --context ApplicationDbContext
```

Apply database updates:

```bash
dotnet ef database update --project src/Migrators/Migrators.MSSQL --startup-project src/Server.UI --context ApplicationDbContext
```

## Where To Look First

- `README.md` for project overview, setup, and supported workflows
- `docs/` for additional repository documentation
- existing feature folders in `src/Application/Features` and related UI pages in `src/Server.UI`
- `docs/superpowers/` for prior design notes and implementation plans when available

## Expected Output Style

When making changes in this repository:

- favor small, reviewable edits
- explain assumptions when behavior is unclear
- verify builds or tests when the task meaningfully changes behavior
- keep new guidance aligned with the current repository structure instead of generic best practices
