'use strict'
const net = require('net');

const [port, sizeOfMessage, iterations] = process.argv.slice(2)
const options = {
    port: +port,
    greetingLengthMessage: 5,
    iterations: +iterations,
    sizeOfMessage: +sizeOfMessage
}
options.maxContentLength = options.sizeOfMessage * options.iterations + options.greetingLengthMessage
let contentLength = 0

const server = net.createServer((client) => {
    console.log('соединение с клиентом было установленно')

    client.on('close', () => {
        console.log(`общее количество байт: ${contentLength}`)
        console.log('соединение было закрыто')
        console.log()
        contentLength = 0
    })

    client.on('readable', () => {
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

server.listen(options.port, () => {
    console.log(`порт - ${options.port}`)
    console.log(`размер сообщения - ${options.sizeOfMessage}`)
    console.log(`количество запросов - ${options.iterations}`)
    console.log()
    console.log('tcp-сервер: ожидание соединения с клиентом...')
})