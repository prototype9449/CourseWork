'use strict'
const net = require('net');
let lengthContent = 0

const server = net.createServer((client) => {
    console.log('client connected');
    client.on('close', () => {
        lengthContent = 0
        console.log('Connection was closed');
    });
    client.on('readable', function () {
        let chunk
        let received = ''
        while (null !== (chunk = client.read())) {
            received += chunk
        }
        lengthContent += received.length
        received !== '' && client.write(received)
        if (received.indexOf('end') !== -1) {
            console.log(`total: ${lengthContent}`)
            client.end()
        }
    })
});

server.on('error', (err) => {
    throw err;
});
server.listen(1337, () => {
    console.log('"Waiting for a connection...');
});