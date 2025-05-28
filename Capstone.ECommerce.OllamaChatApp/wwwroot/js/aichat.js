const notificationInput = document.getElementById('notificationInput');
const sendNotificationButton = document.getElementById('sendNotificationButton');
const notificationsList = document.getElementById('notificationsList');

// Function to scroll chat messages to the bottom
function scrollToBottom() {
    notificationsList.scrollTop = notificationsList.scrollHeight;
}

// Function to add a message to the chat
function addMessage(text, senderClass) {
    const messageDiv = document.createElement('div');
    messageDiv.classList.add('message', senderClass);
    messageDiv.textContent = text;
    notificationsList.appendChild(messageDiv);
    scrollToBottom();
    return messageDiv; // Return the created div for potential updates
}

// --- SignalR Setup ---
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/aiChatHub") // Make sure this matches your Hub's route
    .withAutomaticReconnect() // Optional: Automatically reconnect on disconnect
    .build();

let currentAssistantMessageDiv = null; // To hold the reference to the current assistant message bubble
let isFirstChunk = true; // Flag to check if it's the first chunk for the current response

// Receive notification from the Hub
connection.on("ReceiveNotification", function (message) {
    if (!currentAssistantMessageDiv) {
        // Fallback: If for some reason currentAssistantMessageDiv is null, create a new one.
        // This should ideally be handled by sendMessage.
        currentAssistantMessageDiv = addMessage("", "assistant");
        isFirstChunk = true; // Reset flag for this new message
    }

    if (isFirstChunk) {
        currentAssistantMessageDiv.textContent = message; // Replace "AI is thinking..." with the first chunk
        isFirstChunk = false;
    } else {
        currentAssistantMessageDiv.textContent += message; // Append streamed text
    }
    scrollToBottom();
});

// New event to signal completion of assistant's response
connection.on("ReceiveNotificationComplete", function () {
    console.log("Assistant response complete.");
    currentAssistantMessageDiv = null; // Reset for the next turn
    isFirstChunk = true; // Reset flag for the next response
});


// Start the connection
connection.start().then(function () {
    console.log("SignalR Connected!");
}).catch(function (err) {
    console.error("SignalR Connection Error: ", err.toString());
});

// --- Send Message Logic ---
function sendMessage() {
    const message = notificationInput.value.trim();
    if (message) {

        // 1. Add user's message to the chat
        addMessage(message, "user");

        // 2. Clear input field
        notificationInput.value = '';

        // 3. Add a placeholder for assistant's response (e.g., "AI is thinking...")
        currentAssistantMessageDiv = addMessage("AI is thinking...", "assistant"); // Store reference
        isFirstChunk = true; // Ensure first chunk flag is set for this new message

        // 4. Send message to SignalR Hub
        connection.invoke("SendNotification", message).catch(function (err) {
            console.error("Error invoking SendNotification: ", err.toString());
            if (currentAssistantMessageDiv) {
                currentAssistantMessageDiv.textContent = "Error: Could not get response."; // Update placeholder on error
            }
            currentAssistantMessageDiv = null; // Reset
            isFirstChunk = true; // Reset flag
        });
    }
}

// Event listeners
sendNotificationButton.addEventListener('click', sendMessage);
notificationInput.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        sendMessage();
    }
});

// Initial scroll to bottom in case there are many pre-existing messages
window.onload = scrollToBottom;