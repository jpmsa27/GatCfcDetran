using GatCfcDetran.Services.ExternInterface;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GatCfcDetran.Services.ExternServices
{
    public class RabbitService(ConnectionFactory connectionFactory) : IRabbitService
    {
        private readonly ConnectionFactory _connectionFactory = connectionFactory;
        private IConnection _connection = null!;
        private IChannel _channel = null!;

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            if (_channel == null || !_channel.IsOpen)
            {
                _channel = await _connection.CreateChannelAsync();
            }
        }

        public async Task PublishAsync(string message)
        {
            await EnsureConnectionAsync();

            await _channel.ExchangeDeclareAsync(exchange: "minha-exchange", type: "direct", durable: true, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "minha-exchange",
                routingKey: "minha-chave",
                mandatory: false,
                body: body
            );
        }

        public async Task<string?> ConsumeMessageAsync()
        {
            await EnsureConnectionAsync();
            var result = await _channel.BasicGetAsync(queue: "minha-fila", autoAck: true);

            if (result == null)
                return null;

            var message = Encoding.UTF8.GetString(result.Body.ToArray());
            return message;
        }
        

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

    }
}
