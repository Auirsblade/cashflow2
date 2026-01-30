# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Cashflow 2 is a multiplayer browser-based board game (inspired by the Cashflow board game). Players select professions, roll dice, move around a board, and manage finances through deals, doodads, and market events.

## Architecture

**Monorepo** with two projects under `Cashflow2/`:

- **Cashflow.API/** — .NET 9 ASP.NET Core backend using SignalR for real-time multiplayer. Game state is stored in-memory (IMemoryCache), no database. Game data (boards, deals, doodads, professions) loaded from JSON files in `Resources/`.
- **Cashflow.Web/** — Vue 3 + TypeScript frontend using Vite, Pinia for state, Tailwind CSS v4, and SignalR client.

**Key data flow:** Frontend ↔ SignalR Hub (`GameHub.cs`) → `GameService.cs` (mutations) → in-memory cache. State changes broadcast to all players in a game group.

**Frontend layers:** Auto-generated API client (`src/apiClient/`, from Swagger) · Pinia store (`gameStateStore.ts`) · SignalR composables (`src/lib/signalR.ts`) · Vue components

**Backend layers:** `GameHub.cs` (SignalR endpoint at `/gameHub`) → `GameService.cs` (business logic) · Entities define domain models with computed financial properties · DTOs for client responses

## Common Commands

### Frontend (`Cashflow2/Cashflow.Web/`)
```bash
npm run dev              # Vite dev server (HTTPS, localhost:5173)
npm run build            # Type-check + production build
npm run type-check       # Vue TSC only
npm run generateModels   # Regenerate TypeScript client from Swagger JSON
```

### Backend (`Cashflow2/Cashflow.API/`)
```bash
docker compose up --build   # Build and run API container (port 5066)
dotnet build                # Build without Docker
dotnet run                  # Run without Docker
```

## Key Conventions

- Frontend uses Vue 3 `<script setup>` Composition API with TypeScript
- Path alias `@/*` maps to `src/*` in the frontend
- Backend uses nullable reference types and implicit usings
- C# entities have computed properties for financial calculations (Income, Expenses, NetIncome)
- Game codes are random alphanumeric strings; games are keyed by code in memory cache
- CORS configured for `localhost:5173` (dev) and `cf2.ashercarlow.com` (prod)
- Board has 24 spaces with types: Payday, Deal, Market, Doodad, Charity, Baby, Downsized
