# Task 2 - Report summarizing changes made

## Approach

1. Set up environment.
1. Remove unused code.
1. Build and run project.
1. Add test coverage based on explicit and inferred requirements.
    - Note: adding test coverage based on *inferred* requirements is not standard practice, but falls within the scope of this coding assignment. In a "real-world" setting, I would seek out requirements clarification from the relevant stakeholders/delegates (e.g. Product Owner, Test Specialist) before adding non-obvious/ambiguous test cases.
1. Red-Green-Refactor.
1. Repeat "Red-Green-Refactor" until satisfied.
1. Identify and implement helpful additive changes (not necessarily new functionality, but including scaling improvements, maintenance improvements, etc. which do not fall under "refactoring" category.)
1. Repeat "Red-Green-Refactor" until satisfied.
1. Identify and implement useful improvements potentially adding new functionality.
1. Repeat "Red-Green-Refactor" until satisfied.
1. Review code deployability.
1. Manual testing/review.
1. Finalize solution and create pull request.

## List of improvements

Please see commit history for a granular, itemized list of improvements.

## Bugs & issues found in initial project

### 404 when retrieving categories

#### Steps

- Input `?`
- Input `c`
- Failure occurs when attempting to retrieve categories (404)

#### Additional notes

- There is a working URL at [`https://api.chucknorris.io/jokes/categories`](https://api.chucknorris.io/jokes/categories) to retrieve categories; seems like the URL the program is building is probably malformed.

#### Thrown from

```csharp
// JsonFeed.cs, line 63
return new string[] { Task.FromResult(client.GetStringAsync("categories").Result).Result };
```

#### Error message

##### Aggregate

```
Exception has occurred: CLR/System.AggregateException
An unhandled exception of type 'System.AggregateException' occurred in System.Private.CoreLib.dll: 'One or more errors occurred.'
 Inner exceptions found, see $exception in variables window for more details.
 Innermost exception 	 System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 ().
   at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
   at System.Net.Http.HttpClient.<GetStringAsyncCore>d__27.MoveNext()
```

##### Inner

```
{System.Net.Http.HttpRequestException: Response status code does not indicate success: 404 ().
   at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
   at System.Net.Http.HttpClient.GetStringAsyncCore(Task`1 getTask)}
```

### No newline when `r` entered

#### Steps

- Input `?`
- Input `r`
- No newline before next instruction (`"Want to use a random name? y/n"`)

#### Additional notes

- This applies to several instructions/prompts; should be handled by a reused wrapper

### No input check when prompting to specify a category

#### Steps

- Input `?`
- Input `r`
- Input `y` (`"Want to use a random name? y/n"`)
- Key is not re-assigned prior to the next prompt (`"Want to specify a category? y/n"`)

### "Enter" key can cause parsing exceptions / Inconsistent `ReadKey()` and `ReadLine()` input

#### Steps

- Input `?`
- Input `r`
- Input `y` (`"Want to use a random name? y/n"`)
- Input `Enter`
- Parse exception is thrown from LoC prompting for numeric input (`"How many jokes do you want? (1-9)"`)

#### Additional notes

- Caused by inconsistent use of `ReadKey()` and `ReadLine()` (without clarifying instructions).
- Can also be triggered by entering non-numeric input (e.g. `nine` instead of `9`).
- This is bad UX; the next prompt takes a second or two to show. It is unclear whether or not the user should input `Enter` to advance.
- This should be handled both in UX (clarify instructions, use consistent protocol for getting user input) and in code (validate input, handle unexpected input).

### Prompt to enter category doesn't list categories (confusing UX)

#### Steps

- Input `?`
- Input `r`
- Input `y` (`"Want to use a random name? y/n"`)
- Input `1` (`"How many jokes do you want? (1-9)"`)
- Prompt reads `Enter a category;` but does not list valid categories
- Entering an invalid category causes an exception to be thrown

#### Additional notes

- Categories can be retrieved from [`https://api.chucknorris.io/jokes/categories`](https://api.chucknorris.io/jokes/categories).
- Available categories are: `["animal","career","celebrity","dev","explicit","fashion","food","history","money","movie","music","political","religion","science","sport","travel"]`.
- To avoid typos when entering category, it might be better UX to prompt the user to choose a number from a list of numbered categories.
- There is also a typo in the prompt (should be a colon instead of semi-colon).

### Number of requested jokes is ignored

#### Steps

- Input `?`
- Input `r`
- Input `n` (`"Want to use a random name? y/n"`)
- Input `9` (`"How many jokes do you want? (1-9)"`)
- Only one joke is retrieved.

#### Additional notes

- The number is passed to the `JsonFeed` class constructor, but is not assigned to the instance.

### Number of requested jokes is not validated

#### Steps

- Input `?`
- Input `r`
- Input `n` (`"Want to use a random name? y/n"`)
- Input `10` (`"How many jokes do you want? (1-9)"`)
- One joke is retrieved; no error is thrown or written to console.

#### Additional notes

- The number is passed to the `JsonFeed` class constructor, but is not assigned to the instance.

### Press `?` to get instructions is redundant

Prompting the user to show instructions is redundant. This should be replaced with a more informative prompt, providing a title and some basic instructions on launch.

### Four threads launched when retrieving a joke?

#### Steps

- Input `?`
- Input `r`
- Input `n` (`"Want to use a random name? y/n"`)
- Input `1` (`"How many jokes do you want? (1-9)"`)
- Four running threads appear on the call stack.

#### Additional notes

- This might not be an issue, but is worth investigating; it appears, based on the debugger, as though some of these are long-running threads.

### No exit key/instruction/prompt

No prompt for exiting the program, runs indefinitely. A key can/should be specified to exit the program for improved UX.
