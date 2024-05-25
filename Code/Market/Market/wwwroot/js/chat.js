"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
}).build();
//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    console.log("connection started" + connection.connectionId);
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessageToMe", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveMessageToUser", function (user, targetConnectionId, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg + " by " + targetConnectionId;
    var encodedMsg = msg;// + " by " + targetConnectionId;

    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

//$.connection.hub.disconnected(function () {
//    setTimeout(function () {
//        $.connection.hub.start();
//    }, 2000); // Re-start connection after 5 seconds
//});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    var targetConnectionId = document.getElementById("targetConnectionId").value;
//    connection.invoke("SendMessageToUser", user, targetConnectionId, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

//window.addEventListener("load", function () {
//    var serializealerts = connection.invoke("ReadAlerts", (alerts) =>
//    )
//    var alerts = Newtonsoft.Json.JsonConvert.DeserializeObject < List < dynamic >> (serializealerts);
//    alerts.forEach(function (alert) {
//        var li = document.createElement("li");
//        li.textContent = alert;
//        document.getElementById("messagesList").appendChild(li);
//    }
//    )
//});

