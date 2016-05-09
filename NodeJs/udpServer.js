'use strict'
const dgram = require('dgram');
const server = dgram.createSocket('udp4');

const options = {
    portForReceiving: 3334,
    host: "127.0.0.1",
    iterations: 256 * 8
}
const sendMessage = (message, info) => server.send(message, 0, message.length, info.port, info.address);
let lengthContent = 0,
    iterationNumber = 0

server.on('close', () => console.log('Connection was closed for server'))

server.on('message', (message, info) => {
    lengthContent += message.length
    iterationNumber++
    sendMessage(message,info)
    if (iterationNumber > options.iterations) {
        console.log(`total: ${lengthContent}`)
        lengthContent = 0
        iterationNumber = 0
    }
})

server.on('error', (err) => {
    server.close()
    throw err;
})

server.bind(options.portForReceiving, options.host);