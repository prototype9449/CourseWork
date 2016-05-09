'use strict'
const dgram = require('dgram');
const server = dgram.createSocket('udp4');

const portForReceiving = 3334
const host = "127.0.0.1"
const iterations = 256 * 16

let lengthContent = 0,
    i = 0

server.on('close', () => {
    console.log('Connection was closed for server');
})

server.on('message', (message, info) => {
    lengthContent += message.length
    i++
    server.send(message, 0, message.length, info.port, info.address);
    if (i > iterations) {
        console.log(`total: ${lengthContent}`)
        lengthContent = 0
        i = 0
    }
})

server.on('error', (err) => {
    server.close()
    throw err;
})

server.bind(portForReceiving, host);