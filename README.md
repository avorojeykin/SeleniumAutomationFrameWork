# Selenium Automation Framework

The **Selenium Automation Framework** is a console application built to facilitate UI testing using Selenium. This framework provides a structured foundation for automating testing scenarios, featuring a customizable popup menu with various testing modules. The framework supports authentication, Two-Factor Authentication (2FA), Single Sign-On (SSO), file logging, and basic email notifications based on testing module results.

## Features

- **Modular Testing:** Easily add testing modules to the customizable popup menu, enabling efficient scenario-based UI testing.

- **Authentication Infrastructure:** Built-in support for handling login authentication, Two-Factor Authentication (2FA), and Single Sign-On (SSO) mechanisms.

- **File Logging:** Capture detailed logs of test execution, providing insights into test progress and potential issues.

- **Email Notifications:** Basic email notifications based on testing module results, facilitating quick awareness of test outcomes.

## Usage

1. Clone this repository to your local machine.

2. Ensure you have the Selenium WebDriver installed for Chrome and that Chrome updates are disabled to avoid inconsistent driver updates.

3. Customize the testing modules and authentication logic to match your specific application's requirements. Fine-tune the code to handle your site's HTML and CSS for authentication and login.

4. Build the solution and run the console application.

5. Use the popup menu to select and execute testing modules.

## Important Notes

- The framework provides a basic structure for UI testing, but customization is required to adapt it to your application's specific authentication and testing scenarios.

- Future Chrome updates need to be disabled to maintain compatibility with the Selenium WebDriver.

- This framework is designed with integration into build pipelines in mind, allowing developers to add and execute UI tests as part of the development process.

---

**Note:** This framework requires customization to match your application's authentication mechanism and specific testing scenarios. It is recommended for integration into build pipelines for efficient and automated UI testing.
