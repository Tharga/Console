## Session Continuity

### Starting a session
1. Run `git status` to check for uncommitted changes
   - If uncommitted changes exist, alert me immediately and stop
   - Do not proceed until I have confirmed how to handle them (commit, stash, or discard)
2. Check if `.claude/mission.md` exists and read it.
   - Follow **all** file references listed under "External References" — read each one (shared-instructions, backlog, incoming requests, etc.)
   - If a path uses `$DOC_ROOT`, resolve it via the environment variable defined in `~/.claude/settings.json`.
3. Check if `plan/` exists in the project root.
   - If `plan/plan.md` exists, summarize what has been done and what the next step is.
   - If `plan/feature.md` exists, read the current feature scope.
   - If neither exists, ask me how I would like to proceed.

### During a session
After completing each step in the plan:
- Mark it as `[x]` done in `plan/plan.md`
- Add a brief note about what was done and any important decisions made
- Mark the next step as `[~]` in progress

### Ending a session
- Update `plan/plan.md` with the current status of all steps
- Add a "Last session" note summarizing what was completed and what comes next
- Note any README.md changes that will be needed when the feature is complete

