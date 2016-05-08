'use strict'
const net = require('net');
const sizeOfMessage = 65536*8
const iterations = 128
const message = "1".repeat(sizeOfMessage)

let received = 0
const client = new net.Socket();

let chunkNumber = 0
let time
let isNotRaised = true

client.on('readable', () => {
    let chunk;
    while (null !== (chunk = client.read())) {
        received += chunk.length
    }
    chunkNumber++
    chunkNumber <= iterations && client.write(message)
    if(chunkNumber > iterations && isNotRaised) {
        client.write('end')
        isNotRaised = false
    }
})

client.on('end', () => {
    let diff = process.hrtime(time);
    console.log(`total: ${received}`);
    console.log('work time - %d ms', (diff[0] * 1e9 + diff[1])/1000000);
    console.log('Connection was closed');
})

client.connect(1337, '127.0.0.1', () => {
    console.log('client connected');
    time = process.hrtime()
    client.write(message)
})
