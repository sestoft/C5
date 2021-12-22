# Contributing to C5

First of all, thank you for taking your time to contribute to C5 :tada:

Contributing to C5 is really simple. Whatever you want to do with C5, just look up the appropriate section and follow the steps.

:busts_in_silhouette: But first, please take a look at our [Code of Conduct](CODE-OF-CONDUCT.md).

## Issues

### :warning: C5 doesn't compile / problems with versions

1. Create [a compatibility issue](https://github.com/sestoft/C5/issues/new?labels=compatibility&template=compatibility_issue.md).
1. Write a concise title, as descriptive as possible.
1. Describe your problem. It is really important that you tell us your:

    - **C5 version**.
    - **Visual Studio version** (VS Code, VS 2019...).
    - **Target framework** (.NET 5.0). To find it, right click on your solution and _Properties_.
    - **Operating System**, specially if you are using Mono in a Unix environment.
    - **Unity version**, in case you are using that platform.
    - Any **error** displayed on the console log.

1. Wait until getting some answer back! If you can solve it by yourself, please, publish and document your solution. Contributions as pull requests thereafter are welcome.

### :bug: Hey, bug detected!

1. Create [a bug report issue](https://github.com/sestoft/C5/issues/new?labels=bug&template=bug_report.md).
1. Write a concise title, as descriptive as possible.
1. Make sure that your problem is a bug.
    - If you are struggling with **setting up** an environment with C5, please look at the [previous section](#warning-c5-doesnt-compile--problems-with-versions).
    - If you are requesting **new** functionalities or **enhancing** existing core, please look at the [next section](#bulb-missing-functionalities--improving-existing-functionalities).
1. Describe your problem.
    - Enumerate the steps to reproduce the problem.
    - Using minimum-working examples of code (potentially less than 10 lines) is a good way to explain the issue.
    - Try to identify the clashing classes or files.
1. In case you are willing to solve the problem, see section [Contributing](#contributing).

### :bulb: Missing functionalities / Improving existing functionalities

1. Create [a feature request issue](https://github.com/sestoft/C5/issues/new?labels=enhancement&template=feature_request.md).
1. Write a concise title, as descriptive as possible.
1. Explain what you think that is missing in C5: explain the feature and the solution and its alternatives you've thought about.

### :question: Questions

1. Create a [question issue](https://github.com/sestoft/C5/issues/new?labels=question&template=question.md).
1. Ask your question. That's it!

## Contributing

1. [Fork](https://guides.github.com/activities/forking/) the repository.
1. Make your changes (commits) until you're done.
1. Run all tests (Ctrl+R+A). If any fails, solve your bug and/or refactor your code.
1. When all tests are green, commit your changes. Write a descriptive commit message. _Go back to step 2 as much as you want._
1. Push your changes.
1. Create [a pull request](https://help.github.com/en/articles/creating-a-pull-request). Write a little description of your changes.
