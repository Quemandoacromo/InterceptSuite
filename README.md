<div align="center">

<img src="logo.png" alt="InterceptSuite Logo" width="120">

# InterceptSuite

### TCP/UDP/DTLS/TLS Traffic Interception & Analysis
*MITM proxy for IoT devices, thick clients, and real-time applications*

![Proxy History Tab](Images/Prxoy-History.png)

[![Downloads](https://img.shields.io/github/downloads/anof-cyber/InterceptSuite/total?style=for-the-badge&logo=github&logoColor=white)](https://github.com/anof-cyber/InterceptSuite/releases)
[![Stars](https://img.shields.io/github/stars/anof-cyber/InterceptSuite?style=for-the-badge&logo=github&logoColor=white)](https://github.com/anof-cyber/InterceptSuite)
![Platform Support](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-purple?style=for-the-badge&logo=windows&logoColor=white)
[![Documentation](https://img.shields.io/badge/Documentation-doc.interceptsuite.com-28a745?style=for-the-badge&logo=gitbook&logoColor=white)](https://doc.interceptsuite.com)
[![Get Professional](https://img.shields.io/badge/Get_Professional-Buy_Now-blue?style=for-the-badge&logo=stripe)](https://interceptsuite.com/pricing)

</div>

---

## Overview

**InterceptSuite** is a cross-platform network traffic interception tool for TCP, UDP, DTLS, and TLS protocols. Operating at the transport layer, it provides comprehensive visibility into encrypted traffic from IoT devices, thick clients, databases, gaming applications, and custom protocols.

### Key Capabilities

- **Any Protocol** - Intercepts traffic from any TCP/UDP/TLS/DTLS application
- **IoT & Embedded Devices** - Perfect for analyzing IoT device TCP/UDP  communications
- **Custom Protocols** - Works with thick clients, games, and proprietary protocols
- **Python Extensions** - Add custom protocol dissection capabilities
- **Cross-Platform** - Native support for Windows, Linux, and macOS


## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Upgrade to Professional](#upgrade-to-professional)
- [When to Use InterceptSuite](#when-to-use-interceptsuite-vs-other-specific-tools)
- [Screenshots](#screenshots--interface)
- [Python Extensions](#protocol-dissection--python-extensions)
- [Contributing](#contributing)
- [License](#license)

---

## Features

| Feature | Standard (Free) | Professional |
|:--------|:---------------:|:------------:|
| **Core Features** | | |
| TLS/SSL Interception | ✅ | ✅ |
| TCP & UDP Support | ✅ | ✅ |
| SOCKS5 Proxy | ✅ | ✅ |
| Real-time Traffic Analysis | ✅ | ✅ |
| Connection Management | ✅ | ✅ |
| CA Certificate Management | ✅ | ✅ |
| Traffic Manipulation | ✅ | ✅ |
| Python Extension API | ✅ | ✅ |
| Cross-Platform GUI | ✅ | ✅ |
| High-Performance C Core | ✅ | ✅ |
| **Professional Features** | | |
| DTLS Support (IoT & Real-time) | ❌ | ✅ |
| ANY TCP to TLS like STARTTLS | ❌ | ✅ |
| PCAP Export | ❌ | ✅ |
| Project File Management | ❌ | ✅ |
| **Licensing** | | |
| System Limit | Unlimited | Up to 3 |
| Support | Community | Email |
| Price | Free & Open Source | [$35/month or $300/year](https://interceptsuite.com/pricing) |


## Getting Started

### System Requirements
- Windows 10/11 (64-bit), Linux (x64), or macOS 13+ (Apple Silicon)

### Installation

1. Download the installer from the [Releases page](https://github.com/anof-cyber/InterceptSuite/releases)
   - Windows: `.exe` installer
   - Linux: `.deb` or `.rpm` package
   - macOS: `.pkg` installer
2. Run the installer and follow the setup wizard
3. Launch InterceptSuite from your applications menu

## Usage

For installation guides, configuration tutorials, and detailed usage instructions:

- 📚 **[Documentation](https://doc.interceptsuite.com)** - Complete setup and usage guides
- 🎥 **[YouTube Channel](https://www.youtube.com/@InterceptSuite)** - Video tutorials and demonstrations

---

## Upgrade to Professional

Unlock advanced features for professional security testing and IoT analysis:

### Professional Features
- ✅ **DTLS Support** - Decrypt IoT and real-time communication protocols
- ✅ **Universal TLS Upgrade Detection** - Automatic STARTTLS for any TCP protocol (SMTP, IMAP, PostgreSQL, MySQL, etc.)
- ✅ **PCAP Export** - Export captured traffic for Wireshark analysis
- ✅ **Project File Management** - Save and organize your capture sessions
- ✅ **Multi-System License** - Use on up to 3 systems simultaneously

### Pricing
- **Monthly**: $35/month
- **Yearly**: $300/year (Save $120 - 29% off)

### Get Professional

<div align="center">

[![Get Professional](https://img.shields.io/badge/Get_Professional-Buy_Now-blue?style=for-the-badge&logo=stripe)](https://interceptsuite.com/pricing)


[View Pricing Details →](https://interceptsuite.com/pricing)

</div>

---

## When to Use InterceptSuite vs Other Specific Tools

While InterceptSuite can handle HTTP/HTTPS traffic, **use HTTP-focused tools like Burp Suite, ZAP, Fiddler, Caido, or HTTP Toolkit for web application testing**. These tools provide specialized features optimized for HTTP protocols.

### Use InterceptSuite for:

- **IoT Devices** - MQTT, CoAP, custom IoT protocols over TCP/UDP/DTLS
- **Thick Client Applications** - Desktop apps with custom protocols
- **Database Connections** - PostgreSQL, MySQL, MSSQL over TLS
- **Real-time Applications** - Gaming protocols, VoIP, video streaming
- **Custom Protocols** - Proprietary TLS/DTLS encrypted protocols
- **Embedded Systems** - Low-level TCP/UDP traffic analysis

### Use HTTP-Focused Tools for:

- Web applications and REST APIs
- Mobile app HTTP/HTTPS traffic
- HTTP-specific features (scanner, repeater, intruder)
- Web security assessments

### Decision Matrix

| Use Case | InterceptSuite | HTTP Tools (Burp/ZAP/Fiddler/etc) |
|:---------|:--------------:|:---------------------------------:|
| Web Application Testing | Limited | ✅ |
| Mobile App API (HTTP) | Limited | ✅ |
| IoT Device Communications | ✅ | ❌ |
| Thick Client Applications | ✅ | ❌ |
| Database TLS Connections | ✅ | ❌ |
| Gaming Protocols | ✅ | ❌ |
| DTLS Traffic | ✅ (Pro) | ❌ |

## 🖼️ Screenshots & Interface
## Screenshots & Interface

### Intercept Tab
![Intercept Tab](Images/Intercept.png)
*View and modify network packets in real-time*

### Proxy History Tab
![Proxy History Tab](Images/Prxoy-History.png)
*View all traffic that has passed through the proxy*

### Settings Tab
![Settings Tab](Images/Settings.png)
*Configure proxy server, logging, and certificate management*

### Connections Tab
![Connections Tab](Images/Connections.png)
*Monitor active connections and session details*

## Protocol Dissection & Python Extensions

InterceptSuite provides a **Python Extension API** for custom protocol dissection. Write your own dissectors for any protocol you encounter.

### Benefits

- **Flexible** - Write dissectors for any protocol
- **Customizable** - Tailor parsing to your specific needs
- **Lightweight** - Load only the dissectors you need
- **Extensible** - Add new protocols without waiting for updates

### Built-in Features

- Raw decrypted traffic viewing
- Connection-level information
- Timestamp analysis

For documentation and examples, visit [doc.interceptsuite.com](https://doc.interceptsuite.com)



## 🤝 Contributing

## Contributing

Contributions are welcome! Please:

- Report bugs via GitHub Issues
- Submit feature requests
- Check contribution guidelines before submitting PRs
- Help improve documentation and examples

## License

This project is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)**

[![License](https://img.shields.io/badge/License-AGPL%20v3.0-blue?style=for-the-badge&logo=gnu)](LICENSE)

---

## Built With

- **OpenSSL** - TLS/SSL functionality
- **Avalonia .NET** - Cross-platform GUI
- **CMake** - Build system

---

<div align="center">

**Secure by Design • Cross-Platform • Open Source**

[Website](https://interceptsuite.com) • [Documentation](https://doc.interceptsuite.com) • [Downloads](https://github.com/anof-cyber/InterceptSuite/releases)

</div>
