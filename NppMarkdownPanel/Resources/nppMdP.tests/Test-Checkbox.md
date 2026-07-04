# Checkbox Toggle Test

## Test Tasks

- [ ] Task 1: Setup project
- [x] Task 2: Create repository
- [ ] Task 3: Write documentation
- [x] Task 4: Run tests
- [ ] Task 5: Deploy to production

## Mixed Content

Here is a paragraph with some text before the list.

- [ ] Download dependencies
- [x] Configure environment
- [ ] Build the application

Some text after the list.

## Nested Items (not supported for toggle)

- [ ] Parent task
  - [ ] Subtask A
  - [x] Subtask B

## Checkbox in Code Block (should not toggle)

```
- [ ] This is code, not a checkbox
- [x] This should not be interactive
```

## Multiple Checkboxes on Same Line

- [ ] First item [ ] should only toggle the first checkbox

## Radio Toggle Test

### Simple Radio Group

- ( ) Option A
- (x) Option B
- ( ) Option C

### Second Radio Group

- ( ) Red
- ( ) Green
- (x) Blue

### Mixed with Checkboxes

- [ ] Checkbox item
- ( ) Radio option 1
- (x) Radio option 2
- [x] Another checkbox
