# Practical workflow prior to commit and push to the PR

- [Practical workflow prior to commit and push to the PR](#practical-workflow-prior-to-commit-and-push-to-the-pr)
  - [Run this before commit](#run-this-before-commit)

## Run this before commit
1. `dotnet format`
2. `git status`
3. `git diff`
4. `dotnet test`
5. `dotnet format --verify-no-changes`

The important part is `git diff` after `dotnet format`.

Do not blindly commit the formatter output. Quickly inspect it first.