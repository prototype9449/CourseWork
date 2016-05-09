'use strict'

const dgram = require("dgram")
const client = dgram.createSocket("udp4")

const options = {
    portForReceiving: 3333,
    portForSending: 3334,
    iterations: 2048,
    sizeOfMessage: 32768,
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
    console.log(`total: ${totalLength}`)
    console.log(`work time - ${(begin * 1e9 + start) / 1000000} ms`)
    console.log('Connection was closed for client')
    process.exit()
})

client.bind(options.portForReceiving, options.host)
time = process.hrtime()
sendMessage(new Buffer('hello'))