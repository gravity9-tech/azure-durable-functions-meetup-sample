# Azure Durable Functions - Support Workflow Notification

Welcome to the **Azure Durable Functions Sample** repository! This repository contains sample code demonstrating the use of Azure Durable Functions for notifying the support team when any issues arise in the connected system.

## Overview

This project implements a notification workflow using Azure Durable Functions. It includes two orchestrators and several activity functions to handle the complete notification process efficiently.

### Key Components

1. **NotifySupportOrchestrator**
    - **Purpose**: Main orchestrator responsible for the entire notification flow.
    - **Trigger**: Activated by `NotifySupportHttpClient` when an alert is received from an external system.
    - **Flow**:
        1. Fetches support contacts using the `GetContactActivity` function.
        2. Invokes the `SendNotificationOrchestrator` to notify the support contacts.

2. **SendNotificationOrchestrator**
    - **Purpose**: Handles sending notifications to the specified support contact.
    - **Retries**: Attempts to send the notification up to 2 additional times after a specified period if a response is not received.
    - **Flow**:
        1. Sends a notification to `SendNotificationActivity.cs` and waits for a response through the `CallbackHttpClient`.
        2. If a response is received, it stops the workflow and returns the information to the main orchestrator.
        3. If no response is received within the specified period, it notifies the main orchestrator, which then retries the notification with the next contact.

### NotifySupportOrchestrator.cs
Contains the main orchestrator function that coordinates the entire support notification workflow.

### SendNotificationOrchestrator.cs
Handles the logic for sending notifications to support contacts and managing retries.

### GetContactActivity.cs
Activity function responsible for fetching the list of support contacts.

### SendNotificationActivity.cs
Activity function responsible for sending notification to the contact.

### NotifySupportHttpClient.cs
HTTP client function that triggers the main orchestrator upon receiving an alert from the external system.

### CallbackHttpClient.cs
HTTP client function that handles responses from support contacts, notifying the orchestrator of the received responses.

## Getting Started

### Prerequisites
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Visual Studio or VS Code](https://code.visualstudio.com/)
- [Azure Subscription](https://azure.microsoft.com/en-us/free/)

### Running Locally
1. Clone the repository:
    ```sh
    git clone https://github.com/gravity9-tech/azure-durable-functions-meetup-sample.git
    cd azure-durable-functions-meetup-sample
    ```
2. Open the project in Visual Studio or VS Code.
3. Ensure all necessary Azure Functions extensions are installed.
4. Run the project locally using the Azure Functions Core Tools:
    ```sh
    func start
    ```

### Deploying to Azure
1. Publish the function app to your Azure subscription:
    ```sh
    func azure functionapp publish <FunctionAppName>
    ```

## Usage

1. **Trigger the Workflow**: The `NotifySupportHttpClient` function should be invoked by the external system when an alert is generated.
2. **Monitor the Workflow**: Use status url that was returned from the HTTP request to monitor the current status of the execution.
3. **Handle Responses**: The `CallbackHttpClient` function processes responses from support contacts and updates the orchestrator accordingly.


---

Thank you for exploring Azure Durable Functions with us at the Gravity9 Meetup! If you have any questions or need further assistance, feel free to reach out.
