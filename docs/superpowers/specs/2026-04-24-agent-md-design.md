# Create AGENT.md Design

## Status

Approved for implementation.

## Summary

Create a root-level `AGENT.md` for AI coding agents working in this repository. The document should be lightweight, practical, and optimized for fast repository onboarding rather than strict enforcement.

## Goals

- Help AI coding agents understand the repository purpose and architecture quickly.
- Provide a reliable map of the solution structure and layer boundaries.
- Recommend safe working habits that align with the current codebase.
- Point agents to the most useful local references for deeper context.
- Recommend `docs/superpowers/` as the preferred place for design and planning artifacts when a task needs them.

## Non-Goals

- Do not turn `AGENT.md` into a contributor guide for humans.
- Do not add strict process gates or policy-heavy instructions.
- Do not reference `OpenSpec`, because this repository no longer uses it.
- Do not overemphasize historical library migrations that are already complete.

## Current Repository Context

- The repository is a Clean Architecture Blazor Server solution template.
- The main source layout is split across `src/Application`, `src/Domain`, `src/Infrastructure`, `src/Migrators`, and `src/Server.UI`.
- Tests are organized under `tests/*`.
- Design and planning artifacts already exist under `docs/superpowers/`.
- `README.md` is the main project overview and setup entry point.

## Approved Content Shape

The new `AGENT.md` should include:

- A short statement of purpose for AI coding agents.
- A concise project overview.
- A solution map with brief responsibilities for each major folder.
- Working guidance that encourages preserving layer boundaries and reusing existing patterns.
- Common commands for build, test, run, and EF Core migration work.
- A references section pointing to `README.md`, `docs/`, and `docs/superpowers/`.

## Tone

- Direct and compact.
- Helpful without being verbose.
- Guidance-oriented rather than rule-heavy.

## Success Criteria

The work is complete when:

- `AGENT.md` exists at the repository root.
- The file is useful to an AI coding agent with no prior context.
- It reflects the current repository structure.
- It does not mention `OpenSpec`.
- It recommends `superpowers` artifacts only as a helpful workflow, not as a hard requirement.
