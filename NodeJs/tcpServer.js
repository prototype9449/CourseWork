'use strict'
const net = require('net');

const options = {
    maxContentLength: 32768 * 2048 + 5
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
        let chunk,
            buffer = null

        while (null !== (chunk = client.read())) {
            buffer = buffer === null
                ? Uint8Array.from(chunk)
                : Uint8Array.concat(buffer, chunk)
        }
        buffer !== null && (contentLength += buffer.length)
        if (contentLength < options.maxContentLength) {
            return client.write(new Buffer(buffer))
        }

        buffer !== null && client.end(new Buffer(buffer))
    })
})

server.on('error', (err) => {
    server.close()
    throw err;
})

server.listen(1337, () =>  console.log('"Waiting for a connection...'))