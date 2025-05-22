"use strict";

//To build the connection with this notificationsHub
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationsHub").build();

//Disable the send button until connection is established.
document.getElementById("sendNotificationButton").disabled = true;

connection.on("ReceiveNotification", function (notification) {
    //var li = document.createElement("li");    
    document.getElementById("notificationsList").append(notification);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    //li.textContent = `${notification}`;
});

//Enable the button when connection is established.
connection.start().then(function () {
    document.getElementById("sendNotificationButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendNotificationButton").addEventListener("click", function (event) {
    console.log("TEST")
    document.getElementById("notificationsList").innerHTML = "";
    var notification = document.getElementById("notificationInput").value;
    connection.invoke("SendNotification", notification).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});