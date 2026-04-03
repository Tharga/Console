## Session Continuity

### Starting a session
1. Run `git status` to check for uncommitted changes
   - If uncommitted changes exist, alert me immediately and stop
   - Do not proceed until I have confirmed how to handle them (commit, stash, or discard)
2. Check if `.claude/mission.md` exists and read the project mission and context.
3. Read the project's plan directory (see mission.md for location).
   - If `plan.md` exists, summarize what has been done and what the next step is.
   - If `feature.md` exists, read the current feature scope.
   - If neither exists, ask me how I would like to proceed.

### During a session
After completing each step in the plan:
- Mark it as `[x]` done in `plan.md`
- Add a brief note about what was done and any important decisions made
- Mark the next step as `[~]` in progress

### Ending a session
- Update `plan.md` with the current status of all steps
- Add a "Last session" note summarizing what was completed and what comes next
- Note any README.md changes that will be needed when the feature is complete

## Testing Rules
- Run relevant tests after completing each step before marking it done
- If tests fail, fix the issue before moving on — do not proceed with a failing test
- Run the full test suite before any git commit
- If no tests exist for the code being changed, write them first before implementing

## Build & Test
```bash
dotnet build -c Release
dotnet test -c Release
```

## Coding Guidelines
- Write tests for every new function
- Extract shared or repeated strings into named constants
- Suggest bumping of minor version when compatibility is broken
- Prefer functional programming patterns
- Prefer the command pattern for operations and side effects
- Use `init` over `set` wherever possible

### Feature and framework organization
- Place feature-specific code under `features/[name]`
- Place shared cross-functional code under `framework/[name]`
- Use `framework/` only for code that is reused across multiple features
- If code primarily belongs to one feature, keep it under that feature even if it has some reuse potential
- When in doubt, prefer `features/[name]` over `framework/[name]`
- Do not introduce shared abstractions in `framework/` prematurely; promote code there only when cross-feature reuse is clear

## Workflow Rules
- Before making changes, explain what you plan to do
- After completing a task, summarize what was changed
- If unsure about something, ask before proceeding
- **Cross-project guard:** If an instruction or change targets a different project than the one currently open, ask the user for confirmation before proceeding. Do not silently apply changes to other projects.

## Git Rules
- Never push to remote without explicit approval from me
- Never force push under any circumstances
- Create branch `feature/<feature-name>` at the start of each feature
- Commit at logical milestones (e.g. a component is complete and tested)
- Never commit failing tests
- Use conventional commits: `feat:`, `fix:`, `test:`, `docs:`
- Never merge to main — leave that for me to review and merge
- When merging a completed feature back to the originating branch, use `--no-ff` (no fast-forward) to preserve the feature branch history as a merge commit

## Feature Workflow

Feature planning and tracking is stored outside the git repo to avoid unnecessary commits and permission prompts. The plan directory location is defined in `.claude/mission.md` under "Plan directory".

### Starting a feature
When told to start a new feature:
1. Ask for the feature name and goal if not provided
2. Note the current branch as the originating branch for the feature
3. Create a new branch: `git checkout -b feature/<feature-name>`
4. Create `feature.md` in the plan directory with goal, scope, acceptance criteria, and done condition
5. Create or update `plan.md` in the plan directory with the steps to implement the feature
6. Confirm the plan before starting any code changes

### During implementation
- Update `plan.md` continuously as changes are made
- Commit to the feature branch at logical milestones
- Run tests before each commit

### Completing implementation
When all planned steps are done:
- All tests pass
- Commit all changes
- Summarize what was done and ask the user to test and provide feedback
- Do NOT close the feature — wait for the user to confirm it is done

### Closing a feature (only when the user says it is done)
- All acceptance criteria in `feature.md` are met
- All tests pass
- README.md has been updated to reflect the new feature
- Archive `feature.md` to a `done/` subdirectory in the plan directory and delete `plan.md`
- A final commit is made with message: `feat: <feature-name> complete`
- Merge to originating branch with `--no-ff` and delete feature branch only when the user explicitly asks

## Feature Requests (cross-project)

Cross-project requests are handled via `mission.md` — see the "Incoming requests" reference there for the central location.

- On startup, check `mission.md` for the requests location and show pending requests to the user
- Writing feature requests is **exempt from the cross-project guard**
- Never mark a request as done without user confirmation
- When a request is completed: update its status to Done in the central file, add completion date and summary

## Backlog Hygiene
- When a task from the backlog (in `mission.md` or linked external files) is completed, mark it as done or remove it
- When fixing a bug listed in the backlog, remove the bug entry after the fix is verified
- Keep the backlog current — do not leave completed items lingering
