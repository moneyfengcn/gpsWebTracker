using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTrackerMQ.Base
{
    public enum MQ_WorkMode : int
    {
        /// <summary>
        /// 每个消费者轮流获得一条消息
        /// </summary>
        顺序模式 = 0,
        /// <summary>
        /// 所有消费都能收到同一条消息
        /// </summary>
        群发模式 = 1
    }

    public enum SubscribeType : int
    {
        生产者 = 0,
        消费者
    }
    public class EventBus<T> : IEventBus<T>, IDisposable
    {
        private string _connectionString = "";
        private string _exchangeName = "";
        private string _queueName = "";
        private string _routingKey = "";

        private RabbitMQ.Client.ConnectionFactory _factory = null;
        private RabbitMQ.Client.IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private Func<T, bool> _callback = null;
        private bool disposedValue;

        public EventBus(string connectionString, string exchangeName, string queueName, MQ_WorkMode workMode, SubscribeType subscribeType)
        {
            _connectionString = connectionString;

            _factory = new RabbitMQ.Client.ConnectionFactory();
            _factory.Uri = new Uri(connectionString);
            _connection = _factory.CreateConnection();            
            _channel = _connection.CreateModel();

            if (workMode == MQ_WorkMode.群发模式)
            {
                //定义exchanges
                _channel.ExchangeDeclare(exchangeName, RabbitMQ.Client.ExchangeType.Fanout);

                if (subscribeType == SubscribeType.消费者)
                {
                    //声明队列(临时)
                    queueName = _channel.QueueDeclare().QueueName;
                    //绑定exchanges 和Queues
                    _channel.QueueBind(queueName, exchangeName, _routingKey, null);
                }
            }
            else
            {
                //定义exchanges
                _channel.ExchangeDeclare(exchangeName, RabbitMQ.Client.ExchangeType.Direct);
                ////声明队列
                _channel.QueueDeclare(queueName, false, false, false, null);
                //绑定exchanges 和Queues
                _channel.QueueBind(queueName, exchangeName, _routingKey, null);


            }
            _queueName = queueName;
            _exchangeName = exchangeName;

        }

        protected void SendMessage(T message)
        {
            var json = System.Text.Json.JsonSerializer.Serialize<T>(message);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_exchangeName, _routingKey, null, body); //开始传递
        }
        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="callback"></param>
        public void Register(Func<T, bool> callback)
        {
            if (_callback == null)
            {
                _callback = callback;
                //订阅方式获取message
                var consumer = new EventingBasicConsumer(_channel);

                //autoAck 主动应答false
                _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

                //实现获取message处理事件
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    var message = System.Text.Json.JsonSerializer.Deserialize<T>(json);
                    var ok = _callback(message);
                    if (ok)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }

                };
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                if (_channel != null)
                {
                    _channel.Dispose();
                    _channel = null;
                }
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
                if (_factory != null)
                {
                    _factory = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~EventBus()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
