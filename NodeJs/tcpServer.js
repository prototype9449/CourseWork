'use strict'
const net = require('net');

const options = {
    maxContentLength: 16384 * 8192 + 5
}
const client = new net.Socket();
let contentLength = 0

const server = net.createServer((client) => {
    console.log('client connected')

    client.on('close', () => {
        console.log(`total server: ${contentLength}`)
        console.log('Connection was closed')
        contentLength = 0
    })

    client.on('readable', function () {
        const buffer = client.read()
        if (buffer !== null)
            contentLength += buffer.length

        if (contentLength < options.maxContentLength) {
            return client.write(buffer)
        }

        buffer !== null && client.end(buffer)
    })
})

server.on('error', (err) => {
    server.close()
    throw err;
})

server.listen(1337, () =>  console.log('"Waiting for a connection...'))