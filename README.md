# TrustedExecuter

**TrustedExecuter** is a Windows GUI tool that allows you to run commands with elevated privileges, such as **`NT AUTHORITY\SYSTEM`**, **`TrustedInstaller`**, or **`Administrator`** rights. The tool provides an easy-to-use graphical interface for executing commands with high-level privileges.

> [!WARNING]
> **TrustedExecuter** allows you to execute commands with very high privileges, which can significantly affect your system.
> Always ensure that the commands you run are trusted, as they may modify sensitive system files and configurations.
> Use this tool responsibly to avoid making unintended or harmful changes to your system.

## Features

- **Run as Administrator**: Run commands with **Administrator** privileges.
- **Run as NT AUTHORITY\SYSTEM**: Run commands with **NT AUTHORITY\SYSTEM** privileges.
- **Run as TrustedInstaller**: Run commands with **TrustedInstaller** privileges.
- **GUI-based**: Execute commands with ease using the graphical user interface, without needing to use the command line.

## Installation

1. Download the **TrustedExecuter** installer from the [official GitHub repository](https://github.com/foldesandras/trustedexecuter).
2. Run the installer and follow the on-screen instructions to complete the installation.

## Usage

After installation, you can run commands with elevated privileges using the graphical interface:

1. Open **TrustedExecuter**.
2. Enter the command you wish to run, for example: `cmd /k echo Hello from SYSTEM`.
3. Choose the desired privilege level from the dropdown:
   - **Run normally**: Run with normal user privileges.
   - **Run as Administrator**: Run with **Administrator** privileges.
   - **Run as NT AUTHORITY\SYSTEM**: Run with **NT AUTHORITY\SYSTEM** privileges.
   - **Run as TrustedInstaller**: Run with **TrustedInstaller** privileges.
4. Click the **Run** button to execute the command with the selected privileges.

### Example

To run a command with **NT AUTHORITY\SYSTEM** privileges:

1. Enter the command: `cmd /k echo Hello from SYSTEM`.
2. Select **Run as NT AUTHORITY\SYSTEM**.
3. Click **Run**.

**TrustedExecuter** will execute the command with **NT AUTHORITY\SYSTEM** privileges.

## Parameters

- **Run normally**: Run with normal user privileges.
- **Run as Administrator**: Run with **Administrator** rights.
- **Run as NT AUTHORITY\SYSTEM**: Run with **NT AUTHORITY\SYSTEM** rights.
- **Run as TrustedInstaller**: Run with **TrustedInstaller** rights.

## License

This tool is open-source and available under the [MIT License](LICENSE).

## Contact

If you have any questions or suggestions, please reach out to us via the GitHub page or open an issue at [TrustedExecuter Issues](https://github.com/foldesandras/trustedexecuter/issues).
