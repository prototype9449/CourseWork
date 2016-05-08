'use strict'
const dgram = require('dgram');
const server = dgram.createSocket('udp4');

const portForReceiving = 3334
const host = "127.0.0.1"
let lengthContent = 0

server.on('close', () => {
    lengthContent = 0
    console.log('Connection was closed for server');
});
server.on('message', (message, info) => {
    lengthContent += message.length
    message !== '' && server.send(message, 0, message.length, info.port, info.address);
    if (message.toString().indexOf('end') !== -1) {
        console.log(`total: ${lengthContent}`)
        const buffer = new Buffer('end')
        server.send(buffer, 0, buffer.length, info.port, info.address);
    }
})
server.on('error', (err) => {
    server.close()
    throw err;
});

server.bind(portForReceiving, host);