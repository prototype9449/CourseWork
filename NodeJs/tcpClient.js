'use strict'
const net = require('net');

const client = new net.Socket();
const options = {
    sizeOfMessage: 32768,
    iterations: 2048
}
const message = Buffer.alloc(options.sizeOfMessage, '1', 'utf-8');

let contentLength = 0,
    iterationNumber = 0,
    time

client.on('readable', () => {
    let chunk;
    while (null !== (chunk = client.read())) {
        contentLength += chunk.length
    }
    iterationNumber++

    iterationNumber <= options.iterations && client.write(message)
})

client.on('end', () => {
    let [begin,end] = process.hrtime(time)
    console.log(`total client: ${contentLength}`)
    console.log(`work time - ${(begin * 1e9 + end) / 1000000} ms`)
    console.log('Connection was closed')
})

client.connect(1337, '127.0.0.1', () => {
    console.log('client connected');
    time = process.hrtime()
    client.write('hello')
})