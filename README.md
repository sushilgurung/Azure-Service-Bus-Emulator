# Azure Bus Simulator - Local Development with Azure Service Bus

## Overview

The Azure Bus Simulator provides a local simulation of Azure Service Bus functionalities, enabling developers to test and debug their applications without needing access to a live Azure environment.

This guide explains how to set up and run the Azure Bus Simulator locally.


## Prerequisites

### 1. Development Environment:
- **Operating System**: Windows 10/11, macOS, or Linux.
- **.NET SDK**: Version 6.0 or later. [Download .NET SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- **Hardware Requirements**: Minimum 2 GB RAM and 5 GB of disk space.

### 2. Tools:
- **IDE**: Visual Studio or Visual Studio Code (recommended for code editing).
- **Docker**: Required to run the emulator as a containerized application.

### 3. Knowledge:
- Familiarity with Azure Service Bus concepts, such as Queues, Topics, and Subscriptions.

## Running the Emulator

#### Using Docker(Linux container)
Microsoft Reference [link](https://learn.microsoft.com/en-us/azure/service-bus-messaging/test-locally-with-service-bus-emulator?tabs=docker-linux-container#tabs=docker-linux-container)

1. Open **PowerShell** or **Command Prompt**.
2. Navigate to the `Emulator` folder:

```shell
docker compose up -d

```
![Image](https://github.com/user-attachments/assets/bcc79af4-6018-4e64-bc2c-df0f93df718b)

#### Use the following connection string to connect to the Service Bus emulator

```
Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;
```


## Components

### 1. Publisher
The Publisher is a web application built with Blazor. It sends messages to the Azure Bus Simulator.

#### Steps to Run:
- Open the Publisher project in Visual Studio or Visual Studio Code.
- Start the application using the IDE or the command line.
- Once running, the application will open in a web browser for testing.

<img width="953" alt="Image" src="https://github.com/user-attachments/assets/d2004154-f191-4dab-9b41-4a3bf62e5c7c" />

### 2. ConsoleReceiver
The ConsoleReceiver listens for and receives messages from the Azure Bus Simulator.
#### Steps to Run:
- Start the ConsoleReceiver project using your IDE or the command line.
- It will begin listening for messages sent by the Publisher.

![Image](https://github.com/user-attachments/assets/80ac009f-7de1-4634-a20c-b4f07866e854)
