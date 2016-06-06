'use strict'
const dgram = require('dgram')
const server = dgram.createSocket('udp4')

const [portForReceiving, iterations] = process.argv.slice(2)
const options = {
    portForReceiving: +portForReceiving,
    host: "127.0.0.1",
    iterations: +iterations
}
const sendMessage = (message, info) => server.send(message, 0, message.length, info.port, info.address)
let lengthContent = 0,
    iterationNumber = 0

server.on('close', () => console.log('соединение было закрыто'))

server.on('message', (message, info) => {
    lengthContent += message.length
    iterationNumber++
    sendMessage(message, info)
    if (iterationNumber > options.iterations) {
        console.log(`общее количество байт: ${lengthContent}`)
        lengthContent = 0
        iterationNumber = 0
    }
})

server.on('error', (err) => {
    server.close()
    throw err;
})

console.log(`количество запросов - ${options.iterations}`)
console.log(`порт для получения сообщений - ${options.portForReceiving}`)
console.log()
console.log('udp-сервер: ожидание сообщений клиента...')

server.bind(options.portForReceiving, options.host)