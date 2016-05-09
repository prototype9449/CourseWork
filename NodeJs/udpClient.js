'use strict'

var dgram = require("dgram")
var client = dgram.createSocket("udp4")

const portForReceiving = 3333
const portForSending = 3334
const host = "127.0.0.1"
const sizeOfMessage = 32768
const iterations = 256 * 16
const message = new Buffer("1".repeat(sizeOfMessage))
let time,
    i = 0,
    totalLength = 0

client.on("message", (msg) => {
   totalLength += msg.length
   i++
   client.send(message, 0, message.length, portForSending, host)
   if (i > iterations) {
      return client.close()
   }
});

client.on('close', () => {
   var diff = process.hrtime(time);
   console.log(`total: ${totalLength}`);
   console.log('work time - %d ms', (diff[0] * 1e9 + diff[1]) / 1000000);
   console.log('Connection was closed for client');
   process.exit()
});

client.bind(portForReceiving, host)
time = process.hrtime()
var buffer = new Buffer('hello');
client.send(buffer, 0, buffer.length, portForSending, host);