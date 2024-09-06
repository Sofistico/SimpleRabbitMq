using System.Configuration;

namespace SimpleRabbitMq.Core.Configuration;

/// <summary>
/// Class that loads the config
/// </summary>
public class RabbitMqConnection : ConfigurationSection
{
    private const string _configName = "RabbitMq";

    public RabbitMqConnection()
    {
    }

    /// <summary>
    /// Host addres of RabbitMQ
    /// </summary>
    [ConfigurationProperty("host", IsRequired = true)]
    public string Host
    {
        get
        {
            try
            {
                return (string)this["host"];
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
        set
        {
            try
            {
                this["host"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// User of RabbitMQ
    /// </summary>
    [ConfigurationProperty("user", IsRequired = true)]
    public string User
    {
        get
        {
            try
            {
                return (string)this["user"];
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
        set
        {
            try
            {
                this["user"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Password of RabbitMQ
    /// </summary>
    [ConfigurationProperty("password", IsRequired = true)]
    public string Password
    {
        get
        {
            try
            {
                return (string)this["password"];
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
        set
        {
            try
            {
                this["password"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Host addres of RabbitMQ
    /// </summary>
    [ConfigurationProperty("virtualhost", IsRequired = false)]
    public string VirtualHost
    {
        get
        {
            try
            {
                var vh = (string)this["virtualhost"];
                return string.IsNullOrEmpty(vh) ? "/" : vh;
            }
            catch
            {
                return null;
            }
        }
        set
        {
            try
            {
                this["virtualhost"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Automatic recovery of connection
    /// </summary>
    [ConfigurationProperty("AutomaticRecoveryEnabled", IsRequired = true)]
    public bool AutomaticRecoveryEnabled
    {
        get
        {
            try
            {
                bool are = (bool)this["AutomaticRecoveryEnabled"];
                return !are ? true : are;
            }
            catch
            {
                return true;
            }
        }
        set
        {
            try
            {
                this["AutomaticRecoveryEnabled"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Topology Recovery Enabled
    /// </summary>
    [ConfigurationProperty("TopologyRecoveryEnabled", IsRequired = true)]
    public bool TopologyRecoveryEnabled
    {
        get
        {
            try
            {
                bool tre = (bool)this["TopologyRecoveryEnabled"];
                return !tre ? true : tre;
            }
            catch
            {
                return true;
            }
        }
        set
        {
            try
            {
                this["TopologyRecoveryEnabled"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Requested Heartbeat
    /// </summary>
    [ConfigurationProperty("RequestedHeartbeat", IsRequired = true)]
    public TimeSpan RequestedHeartbeat
    {
        get
        {
            try
            {
                TimeSpan rh = TimeSpan.FromSeconds(300);
                return rh < (TimeSpan)this["RequestedHeartbeat"]
                    ? (TimeSpan)this["RequestedHeartbeat"]
                    : rh;
            }
            catch
            {
                return TimeSpan.FromSeconds(300);
            }
        }
        set
        {
            try
            {
                this["RequestedHeartbeat"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Network Recovery Interval
    /// </summary>
    [ConfigurationProperty("NetworkRecoveryInterval", IsRequired = false)]
    public TimeSpan NetworkRecoveryInterval
    {
        get
        {
            try
            {
                TimeSpan nri = (TimeSpan)this["NetworkRecoveryInterval"];
                return nri != null ? nri : TimeSpan.FromSeconds(30);
            }
            catch
            {
                return TimeSpan.FromSeconds(30);
            }
        }
        set
        {
            try
            {
                this["NetworkRecoveryInterval"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Continuation Timeout
    /// </summary>
    [ConfigurationProperty("ContinuationTimeout", IsRequired = false)]
    public TimeSpan ContinuationTimeout
    {
        get
        {
            try
            {
                TimeSpan ct = (TimeSpan)this["ContinuationTimeout"];
                return ct != null ? ct : TimeSpan.FromSeconds(30);
            }
            catch
            {
                return TimeSpan.FromSeconds(30);
            }
        }
        set
        {
            try
            {
                this["ContinuationTimeout"] = value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{_configName} must be set inside a configuration source",
                    ex
                );
            }
        }
    }
}
