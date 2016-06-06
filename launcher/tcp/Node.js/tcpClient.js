"use strict"

const net = require('net');
const client = new net.Socket();

const [port, sizeOfMessage, iterations] = process.argv.slice(2)

const options = {
    sizeOfMessage: +sizeOfMessage,
    iterations: +iterations,
    port: +port,
    host: '127.0.0.1'
}
const state = {
    contentLength: 0,
    iterationNumber: 0,
    time: null
}
const message = Buffer.alloc(options.sizeOfMessage, '1', 'utf-8');

client.on('readable', () => {
    let chunk;
    while (null !== (chunk = client.read())) {
        state.contentLength += chunk.length
    }
    state.iterationNumber++

    state.iterationNumber <= options.iterations && client.write(message)
})
client.on('end', () => {
    let [begin,end] = process.hrtime(state.time)
    console.log(`общее количество байт: ${state.contentLength}`)
    console.log(`время ответа - ${(begin * 1e9 + end) / 1000000} ms`)
    console.log('соединение было закрыто')
})
client.connect(options.port, options.host, () => {
    console.log('соединение с клиентом было установленно');
    state.time = process.hrtime()

    client.write('hello')
})

console.log(`размер сообщения - ${options.sizeOfMessage}`)
console.log(`количество запросов - ${options.iterations}`)
console.log(`порт - ${options.port}`)
console.log()
console.log('tcp-клиент: ожидание соединение с сервером...')