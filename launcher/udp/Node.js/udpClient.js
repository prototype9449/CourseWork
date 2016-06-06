'use strict'

const dgram = require("dgram")
const client = dgram.createSocket("udp4")
const [portForReceiving, portForSending, sizeOfMessage, iterations] = process.argv.slice(2)
const options = {
    portForReceiving: +portForReceiving,
    portForSending: +portForSending,
    iterations: +iterations,
    sizeOfMessage: +sizeOfMessage,
    host: "127.0.0.1"
}
const message = Buffer.alloc(options.sizeOfMessage, '1', 'utf-8')
const sendMessage = (message) => client.send(message, 0, message.length, options.portForSending, options.host)
let time,
    i = 0,
    totalLength = 0

client.on("message", (msg) => {
    totalLength += msg.length
    i++
    sendMessage(message)
    i > options.iterations && client.close()
})

client.on('close', () => {
    var [begin, start] = process.hrtime(time)
    console.log(`общее количество байт: ${totalLength}`)
    console.log(`время ответа - ${(begin * 1e9 + start) / 1000000} ms`)
    console.log('соединение было закрыто')
    process.exit()
})

console.log(`размер сообщения - ${options.sizeOfMessage}`)
console.log(`количество запросов - ${options.iterations}`)
console.log(`порт для отправки сообщений - ${options.portForSending}`)
console.log(`порт для получения сообщений - ${options.portForReceiving}`)
console.log()
console.log('udp-клиент: отправка сообщений серверу')

client.bind(options.portForReceiving, options.host)
time = process.hrtime()
sendMessage(new Buffer('hello'))