# FleetOps Commit & PR Conventions

- [FleetOps Commit \& PR Conventions](#fleetops-commit--pr-conventions)
  - [Purpose](#purpose)
  - [Commit Format](#commit-format)
  - [Commit Types](#commit-types)
    - [feat](#feat)
    - [fix](#fix)
    - [test](#test)
    - [refactor](#refactor)
    - [style](#style)
    - [docs](#docs)
    - [chore](#chore)
  - [Choosing the Correct Type](#choosing-the-correct-type)
  - [Scope Guidelines](#scope-guidelines)
  - [Choosing the Scope](#choosing-the-scope)
  - [Writing the Description](#writing-the-description)
  - [Description Rules](#description-rules)
    - [1. Use the imperative mood](#1-use-the-imperative-mood)
    - [2. Describe the result, not the process](#2-describe-the-result-not-the-process)
    - [3. Be specific, but not too detailed](#3-be-specific-but-not-too-detailed)
    - [4. Avoid subjective words](#4-avoid-subjective-words)
    - [5. Avoid file/class names unless they are the point](#5-avoid-fileclass-names-unless-they-are-the-point)
    - [6. Keep it short](#6-keep-it-short)
  - [Commit Body](#commit-body)
  - [Ordinary Commit Examples](#ordinary-commit-examples)
    - [Add tests](#add-tests)
    - [Add feature behavior](#add-feature-behavior)
    - [Fix behavior](#fix-behavior)
    - [Refactor production code](#refactor-production-code)
    - [Formatting and style](#formatting-and-style)
    - [Documentation](#documentation)
    - [Chores](#chores)
  - [Less Ordinary Commit Examples](#less-ordinary-commit-examples)
    - [Test infrastructure](#test-infrastructure)
    - [Behavior fix caused by database constraints](#behavior-fix-caused-by-database-constraints)
    - [Refactor plus tests](#refactor-plus-tests)
    - [New validation rule](#new-validation-rule)
    - [Rename only](#rename-only)
    - [Delete dead code](#delete-dead-code)
    - [Formatting-only changes](#formatting-only-changes)
    - [Update dependencies](#update-dependencies)
  - [Bad vs Better Examples](#bad-vs-better-examples)
    - [Example 1](#example-1)
    - [Example 2](#example-2)
    - [Example 3](#example-3)
    - [Example 4](#example-4)
    - [Example 5](#example-5)
  - [Mental Lint Test](#mental-lint-test)
    - [1. Does it match the format?](#1-does-it-match-the-format)
    - [2. Is the type correct?](#2-is-the-type-correct)
    - [3. Is the scope useful?](#3-is-the-scope-useful)
    - [4. Does the description complete "this commit will..."?](#4-does-the-description-complete-this-commit-will)
    - [5. Does it describe the result instead of the activity?](#5-does-it-describe-the-result-instead-of-the-activity)
    - [6. Is it specific enough?](#6-is-it-specific-enough)
    - [7. Is it free from subjective words?](#7-is-it-free-from-subjective-words)
    - [8. Is it short enough?](#8-is-it-short-enough)
  - [Quick Commit Message Checklist](#quick-commit-message-checklist)
  - [Pull Request Titles](#pull-request-titles)
  - [Squash Merge Message](#squash-merge-message)
  - [Recommended FleetOps Defaults](#recommended-fleetops-defaults)
  - [Final Rule](#final-rule)

## Purpose

This document defines a lightweight commit and pull request naming standard for FleetOps.

The goal is not to make commits complicated. The goal is to make Git history useful.

A good commit message should help a future reader understand:

- what changed
- where it changed
- why the change exists
- whether the change added behavior, fixed behavior, improved tests, cleaned structure, or normalized formatting

---

## Commit Format

Use this format:

```text
type(scope): short description
```

If a commit affects the whole codebase and no scope adds clarity, the scope may be omitted:

```text
type: short description
```

Examples:

```text
feat(assignments): add overlap validation
fix(assignments): return conflict for overlapping vehicles
test(assignments): add pagination integration tests
refactor(database): extract DbSeedBuilder
style: apply dotnet format
docs(readme): add local setup instructions
chore(deps): update xunit packages
```

---

## Commit Types

### feat

Use `feat` when adding new functionality or application behavior.

A feature means the system can now do something it could not do before.

Examples:

```text
feat(assignments): add assignment creation endpoint
feat(assignments): add filtering by driver
feat(vehicles): add vehicle registration endpoint
```

Use `feat` when:

- a new endpoint is added
- a new query/filter option is added
- a new user-visible behavior is added
- the API supports a new use case

Do not use `feat` for tests, formatting, cleanup, or internal restructuring.

---

### fix

Use `fix` when correcting incorrect behavior.

Examples:

```text
fix(assignments): prevent overlapping driver assignments
fix(assignments): return conflict for overlapping vehicles
fix(drivers): return not found for missing driver
fix(api): map database conflicts to error responses
```

Use `fix` when:

- the previous behavior was wrong
- the API returned the wrong status code
- the API returned the wrong error code
- invalid data was accepted
- valid data was rejected
- an exception escaped incorrectly
- behavior contradicted the intended rules

A fix does not need to be large. A one-line correction can still be a `fix`.

---

### test

Use `test` when the primary purpose is adding, changing, or improving tests.

Examples:

```text
test(assignments): add filtering integration tests
test(assignments): verify ordering and complete response payload
test(drivers): add validation error assertions
test(integration): introduce DbSeedBuilder
test(vehicles): assert created response body
```

Use `test` when:

- adding new tests
- improving existing assertions
- adding test helpers
- adding seed builders
- making tests more deterministic
- improving test coverage

Important:

If the change mainly improves test code, prefer `test`, even if the implementation feels like a refactor.

Good:

```text
test(integration): introduce DbSeedBuilder
```

Less good:

```text
refactor(tests): introduce DbSeedBuilder
```

Reason: the purpose is to improve the test suite.

---

### refactor

Use `refactor` when changing code structure without intentionally changing behavior.

Examples:

```text
refactor(assignments): extract overlap validation
refactor(api): move exception mapping to middleware
refactor(database): simplify seeding logic
refactor(application): reduce handler duplication
```

Use `refactor` when:

- behavior should stay the same
- code is moved, renamed, split, or simplified
- duplication is reduced
- responsibilities are clarified
- maintainability improves

Do not use `refactor` if the observable behavior changes. If the API response, status code, validation rule, or business behavior changes, it is probably `feat` or `fix`.

---

### style

Use `style` when changing formatting only.

A style change does not intentionally change behavior, tests, or code structure. It only changes how the code is laid out.

Examples:

```text
style: apply dotnet format
style(solution): normalize whitespace
style(api): format endpoint definitions
```

Use `style` when:

- running `dotnet format`
- changing whitespace
- changing indentation
- removing or adding blank lines
- normalizing line breaks
- sorting or cleaning using directives as part of formatting

Do not use `style` when:

- code is moved, split, extracted, or renamed; use `refactor`
- tooling or CI configuration changes; use `chore`
- documentation text changes; use `docs`
- observable behavior changes; use `feat` or `fix`

Important:

`style` means code formatting style. It does not mean visual UI styling. If a UI appearance changes, choose the type based on whether that is a new feature or a bug fix.

---

### docs

Use `docs` when changing documentation only.

Examples:

```text
docs(readme): add local PostgreSQL setup
docs(contributing): add commit conventions
docs(api): document error response format
```

Use `docs` when:

- only documentation changes
- README is updated
- contribution guidelines are added
- setup instructions are clarified

If code changes too, choose the type based on the code change.

---

### chore

Use `chore` for maintenance work that does not fit the other types.

Examples:

```text
chore(deps): update Shouldly
chore(ci): cache NuGet packages
chore(github): add pull request template
chore(solution): remove unused project reference
```

Use `chore` when:

- updating dependencies
- adjusting CI configuration
- changing repository metadata
- cleaning solution/project files
- changing tooling configuration

Do not use `chore` as a fallback for unclear commits. If the change affects behavior, tests, documentation, or formatting, use the more specific type.

---

## Choosing the Correct Type

Choose the type based on the reason the change exists.

| Question | Type |
|---|---|
| Does the application do something new? | `feat` |
| Was incorrect behavior corrected? | `fix` |
| Is the main purpose test coverage or test infrastructure? | `test` |
| Was code structure changed without behavior changes? | `refactor` |
| Is it formatting or whitespace only? | `style` |
| Is it documentation only? | `docs` |
| Is it maintenance/tooling/dependencies? | `chore` |

When in doubt, ask:

> What would I tell another developer this commit mainly does?

---

## Scope Guidelines

The scope identifies the main area affected.

Recommended FleetOps scopes:

```text
assignments
drivers
vehicles
api
application
domain
infrastructure
database
integration
unit
ci
deps
docs
github
solution
```

Examples:

```text
feat(assignments): add date range filtering
fix(api): return validation error for invalid query
test(integration): add status code assertions
refactor(database): simplify seed builder
chore(deps): update xunit packages
style: apply dotnet format
docs(readme): add database setup instructions
```

---

## Choosing the Scope

Choose the most meaningful area, not every area touched.

Good:

```text
fix(assignments): return conflict for overlapping drivers
```

Avoid:

```text
fix(api-database-validation-assignment-overlap): return conflict for overlapping drivers
```

Keep the scope short.

If a change touches many areas, choose the main reason for the change.

Examples:

```text
feat(assignments): add creation endpoint
```

This may touch API, application, domain, infrastructure, and tests. The main feature is still assignments.

```text
test(integration): introduce DbSeedBuilder
```

This may touch many test files, but the main area is integration test infrastructure.

For formatting-only commits that touch many areas, either omit the scope or use a broad scope such as `solution`.

Good:

```text
style: apply dotnet format
```

Also acceptable:

```text
style(solution): normalize whitespace
```

Avoid:

```text
style(application): apply dotnet format
```

if the change touched API, application, domain, infrastructure, and tests. In that case, `application` is misleading because it is also a specific FleetOps layer.

---

## Writing the Description

The description is the most important part of the commit message.

Use an imperative verb phrase.

> Imperative verbs create an imperative sentence (i.e., a sentence that gives an order or command). When you read an imperative sentence, it will often sound like the speaker is telling someone what to do, even if the sentence has a polite tone. Imperative verbs don’t leave room for questions or discussion.
> Ex: "Walk the dog" or "Make me a pizza"

Think of the message as completing this sentence:

> This commit will...

Good:

```text
test(assignments): verify ordering and complete response payload
```

This commit will verify ordering and complete response payload.

Good:

```text
fix(assignments): reject overlapping vehicle assignments
```

This commit will reject overlapping vehicle assignments.

Less good:

```text
test(assignments): improving AssignmentsTest to better test order
```

This is weaker because:

- `improving` is vague
- `better` is subjective
- `AssignmentsTest` is an implementation detail
- it describes the activity more than the result

---

## Description Rules

### 1. Use the imperative mood

Prefer:

```text
add
fix
verify
reject
return
extract
remove
simplify
document
update
```

Avoid:

```text
added
fixed
verified
rejecting
improving
changed
worked on
```

Good:

```text
test(assignments): verify ordering and complete response payload
```

Avoid:

```text
test(assignments): verified ordering and complete response payload
```

---

### 2. Describe the result, not the process

Good:

```text
test(assignments): verify ordering and complete response payload
```

Avoid:

```text
test(assignments): improve tests
```

Good:

```text
refactor(database): extract reusable seed builder
```

Avoid:

```text
refactor(database): move some code around
```

---

### 3. Be specific, but not too detailed

Good:

```text
fix(assignments): return conflict for overlapping vehicles
```

Too vague:

```text
fix(assignments): fix bug
```

Too detailed:

```text
fix(assignments): change postgres exception switch case for ex_assignments_vehicle_no_overlap to return 409 conflict
```

The commit body can contain details if needed.

---

### 4. Avoid subjective words

Avoid:

```text
better
cleaner
improved
proper
nice
more correct
```

Usually these can be replaced with the actual result.

Instead of:

```text
test(assignments): improve assignment tests
```

Use:

```text
test(assignments): verify ordering and response payload
```

Instead of:

```text
refactor(database): make seeding cleaner
```

Use:

```text
refactor(database): extract reusable seed builder
```

---

### 5. Avoid file/class names unless they are the point

Usually avoid:

```text
test(assignments): improve GetAssignmentsTests
```

Prefer:

```text
test(assignments): verify filtering and pagination results
```

Mention the class/file only when the file itself is the meaningful thing.

Acceptable:

```text
docs(readme): add PostgreSQL setup instructions
```

Acceptable:

```text
chore(solution): remove unused test project reference
```

---

### 6. Keep it short

Aim for roughly 50-72 characters after the type/scope when possible.

Good:

```text
fix(assignments): reject overlapping driver assignments
```

Too long:

```text
fix(assignments): reject assignments when the selected driver already has another assignment during the same time range
```

Use a commit body if you need more explanation.

---

## Commit Body

Most commits do not need a body.

Use a body when the reason is not obvious from the title.

Format:

```text
type(scope): short description

Explain why this change was needed.
Explain important tradeoffs or details.
Mention anything future readers should know.
```

Example:

```text
fix(assignments): map overlap violations to conflict responses

PostgreSQL exclusion constraints throw provider-specific exceptions.
The middleware now maps known assignment overlap constraint names to
409 responses so callers receive a stable API-level error.
```

Another example:

```text
test(integration): introduce DbSeedBuilder

The previous tests created entities inline, which made setup noisy and
harder to reuse. The builder centralizes common seed scenarios while
keeping each test responsible for its own expected result.
```

Use the body for:

- why the change was needed
- important design decisions
- tradeoffs
- migration notes
- anything surprising

Do not use the body to repeat the title.

---

## Ordinary Commit Examples

### Add tests

```text
test(assignments): add filtering integration tests
```

```text
test(assignments): verify ordering and complete response payload
```

```text
test(drivers): assert validation error codes
```

```text
test(vehicles): verify created response body
```

---

### Add feature behavior

```text
feat(assignments): add date range filtering
```

```text
feat(drivers): add driver creation endpoint
```

```text
feat(vehicles): support vehicle registration lookup
```

---

### Fix behavior

```text
fix(assignments): reject overlapping driver assignments
```

```text
fix(api): return validation response for invalid queries
```

```text
fix(drivers): return not found for unknown driver
```

---

### Refactor production code

```text
refactor(assignments): extract overlap validation
```

```text
refactor(api): centralize exception mapping
```

```text
refactor(database): simplify entity configuration
```

---

### Formatting and style

```text
style: apply dotnet format
```

```text
style(solution): normalize whitespace
```

```text
style(api): format endpoint declarations
```

---

### Documentation

```text
docs(readme): add local setup instructions
```

```text
docs(contributing): add commit message standard
```

```text
docs(api): describe error response structure
```

---

### Chores

```text
chore(deps): update Shouldly
```

```text
chore(ci): run tests on pull requests
```

```text
chore(solution): remove unused project reference
```

---

## Less Ordinary Commit Examples

### Test infrastructure

```text
test(integration): introduce DbSeedBuilder
```

Use this when adding helper infrastructure used by tests.

---

### Behavior fix caused by database constraints

```text
fix(database): map assignment overlap constraint violations
```

or, if the API behavior is the important part:

```text
fix(api): return conflict for assignment overlap violations
```

Choose `database` if the change is mainly about persistence details.
Choose `api` if the important result is the external response.

---

### Refactor plus tests

If a commit refactors production code and updates tests only because tests had to follow along:

```text
refactor(assignments): extract assignment query handler
```

If the commit mainly improves tests and only touches helper code:

```text
test(integration): simplify assignment seed setup
```

---

### New validation rule

If this adds a new rule that did not exist before:

```text
feat(assignments): reject empty assignment time ranges
```

If this corrects a rule that was supposed to exist already:

```text
fix(assignments): reject empty assignment time ranges
```

The same code change can be either `feat` or `fix` depending on intent.

---

### Rename only

```text
refactor(domain): rename AssignmentPeriod to AssignmentTimeRange
```

Use `refactor` if behavior is unchanged.

---

### Delete dead code

```text
refactor(application): remove unused assignment query
```

or, if it is pure repository cleanup:

```text
chore(solution): remove unused project file
```

---

### Formatting-only changes

Use `style` when the change is only formatting and whitespace.

Good:

```text
style: apply dotnet format
```

Good with body:

```text
style: apply dotnet format

Run dotnet format to normalize whitespace and formatting.

No behavior changes.
```

Avoid:

```text
refactor(application): remove or add whitespace throughout the application
```

Why:

- formatting is not a refactor
- `application` is misleading if many projects or layers changed
- `remove or add whitespace` sounds uncertain

---

### Update dependencies

```text
chore(deps): update Npgsql
```

If the dependency update fixes a known bug, you can use:

```text
fix(deps): update Npgsql to resolve connection issue
```

But only use `fix(deps)` when the purpose is genuinely bug correction.

---

## Bad vs Better Examples

### Example 1

Avoid:

```text
test(integration): improving AssignmentsTest to better test order and entire returned result
```

Better:

```text
test(assignments): verify ordering and complete response payload
```

Why:

- uses imperative phrasing
- removes subjective wording
- describes the result
- avoids unnecessary class name

---

### Example 2

Avoid:

```text
refactor(tests): added SeedBuilder and implemented it in createAssignmentTests
```

Better:

```text
test(integration): introduce DbSeedBuilder
```

Why:

- the primary purpose is test support
- `added` becomes imperative `introduce`
- shorter and clearer

---

### Example 3

Avoid:

```text
fix: fixed stuff with validation
```

Better:

```text
fix(api): return validation errors for invalid requests
```

Why:

- includes scope
- describes observable result
- avoids vague language

---

### Example 4

Avoid:

```text
feat: assignments
```

Better:

```text
feat(assignments): add date range filtering
```

Why:

- says what changed
- says where it changed
- is useful in Git history

---

### Example 5

Avoid:

```text
chore: update
```

Better:

```text
chore(deps): update xunit packages
```

Why:

- identifies what was updated
- uses a meaningful scope

---

## Mental Lint Test

Before committing, read the message and ask these questions.

### 1. Does it match the format?

```text
type(scope): description
```

If not, rewrite it.

---

### 2. Is the type correct?

Ask:

- New behavior? `feat`
- Bug correction? `fix`
- Test coverage or test helper? `test`
- Structure only? `refactor`
- Formatting only? `style`
- Documentation only? `docs`
- Maintenance/tooling? `chore`

---

### 3. Is the scope useful?

The scope should answer:

> Where is the main change?

Good scopes:

```text
assignments
drivers
vehicles
api
database
integration
deps
ci
```

Bad scopes:

```text
misc
stuff
changes
update
```

---

### 4. Does the description complete "this commit will..."?

Good:

```text
test(assignments): verify ordering and complete response payload
```

This commit will verify ordering and complete response payload.

Bad:

```text
test(assignments): improving test stuff
```

This commit will improving test stuff.

That sounds wrong, so rewrite it.

---

### 5. Does it describe the result instead of the activity?

Good:

```text
refactor(database): extract reusable seed builder
```

Bad:

```text
refactor(database): move code around
```

---

### 6. Is it specific enough?

Too vague:

```text
fix(api): fix errors
```

Better:

```text
fix(api): return validation errors for invalid requests
```

---

### 7. Is it free from subjective words?

Watch for:

```text
better
cleaner
improved
proper
nice
good
```

Replace them with the actual result.

---

### 8. Is it short enough?

If the title is getting too long, keep the title short and add a body.

Good title:

```text
fix(assignments): reject overlapping driver assignments
```

Optional body:

```text
The database constraint already prevents this case, but the API now
returns a stable conflict response instead of leaking persistence details.
```

---

## Quick Commit Message Checklist

Before running `git commit`, check:

- [ ] Uses `type(scope): description`
- [ ] Type matches the purpose
- [ ] Uses `style` for formatting-only changes
- [ ] Scope identifies the main area
- [ ] Description uses imperative mood
- [ ] Description says what changed
- [ ] Avoids vague words like `stuff`, `changes`, `fixes`
- [ ] Avoids subjective words like `better`, `cleaner`, `proper`
- [ ] Does not mention file/class names unless useful
- [ ] Has a body if the reason is not obvious

---

## Pull Request Titles

PR titles should usually follow the same format as commits.

Examples:

```text
test(integration): add status code and error code assertions
```

```text
feat(assignments): add assignment filtering
```

```text
fix(assignments): return conflict for overlapping assignments
```

```text
docs(contributing): add commit message standard
```

```text
style: apply dotnet format
```

For a PR with many commits, the PR title should describe the overall change.

Example commits:

```text
test(assignments): add filtering integration tests
test(assignments): verify ordering and complete response payload
test(integration): introduce DbSeedBuilder
```

Good PR title:

```text
test(integration): expand assignment endpoint coverage
```

---

## Squash Merge Message

When squash merging, write the final squash message as if it were one clean commit.

Good:

```text
test(integration): expand assignment endpoint coverage
```

Body:

```text
Adds integration coverage for assignment filtering, pagination, status
codes, error codes, ordering, and full response payloads.
```

Avoid squash messages like:

```text
fix review comments
more tests
cleanup
final changes
```

Those messages are common during development, but they should not become permanent history if you can avoid it.

---

## Recommended FleetOps Defaults

For most FleetOps work, use these patterns:

```text
feat(assignments): ...
fix(assignments): ...
test(assignments): ...
test(integration): ...
refactor(api): ...
refactor(database): ...
style: ...
docs(readme): ...
chore(deps): ...
```

Prefer small, clear commits.

A commit should normally represent one logical change.

If you need the word `and` in the description, check whether the commit should be split.

Example:

```text
test(assignments): add filtering tests and refactor seeding
```

Maybe split into:

```text
test(integration): introduce DbSeedBuilder
test(assignments): add filtering integration tests
```

---

## Final Rule

A good commit message should still make sense six months later when viewed in isolation.

If future you can read the message and immediately understand the purpose of the change, the message is good.
