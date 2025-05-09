# [5.0.0](https://github.com/1SoftwareCompany/Sett1x.Consul/compare/v4.0.0...v5.0.0) (2025-04-08)


### Bug Fixes

* Set up CI with Azure Pipelines ([6b9c045](https://github.com/1SoftwareCompany/Sett1x.Consul/commit/6b9c045f677837161c79d3e6663a9eef0dac02de))

## [4.0.1](https://github.com/Elders/Pandora.Consul/compare/v4.0.0...v4.0.1) (2025-03-25)


### Bug Fixes

* Updates packages ([09b2ae8](https://github.com/Elders/Pandora.Consul/commit/09b2ae8c7eb886753b64bfb9293de122b21c29bd))

# [4.0.0](https://github.com/Elders/Pandora.Consul/compare/v3.3.1...v4.0.0) (2025-03-10)

## [3.3.1](https://github.com/Elders/Pandora.Consul/compare/v3.3.0...v3.3.1) (2025-02-21)


### Bug Fixes

* Skips all broken values when settings are loaded instead of throwing an exception and losing all settings ([14c873c](https://github.com/Elders/Pandora.Consul/commit/14c873c3c8a2b7a6d65f0b35b14c1407504b723b))

# [3.3.0](https://github.com/Elders/Pandora.Consul/compare/v3.2.1...v3.3.0) (2022-12-16)


### Bug Fixes

* Updates dependencies packages ([b5eab35](https://github.com/Elders/Pandora.Consul/commit/b5eab354c5fe4c4f580c1db0fde4877bedc187fc))


### Features

* update Pandora package ([63d5590](https://github.com/Elders/Pandora.Consul/commit/63d5590c1c9465642dca2e7cdf7d9244d319c304))

# [3.3.0-preview.1](https://github.com/Elders/Pandora.Consul/compare/v3.2.2-preview.1...v3.3.0-preview.1) (2022-12-16)


### Features

* update Pandora package ([63d5590](https://github.com/Elders/Pandora.Consul/commit/63d5590c1c9465642dca2e7cdf7d9244d319c304))

## [3.2.2-preview.1](https://github.com/Elders/Pandora.Consul/compare/v3.2.1...v3.2.2-preview.1) (2022-12-16)


### Bug Fixes

* Updates dependencies packages ([b5eab35](https://github.com/Elders/Pandora.Consul/commit/b5eab354c5fe4c4f580c1db0fde4877bedc187fc))

## [3.2.1](https://github.com/Elders/Pandora.Consul/compare/v3.2.0...v3.2.1) (2022-08-16)


### Bug Fixes

* pipeline update ([7908ef5](https://github.com/Elders/Pandora.Consul/commit/7908ef5ec5d96bb4a65e4b64e4f2c6cfce33f065))

# [3.2.0](https://github.com/Elders/Pandora.Consul/compare/v3.1.1...v3.2.0) (2022-07-06)


### Features

* Updates packages ([1c937fc](https://github.com/Elders/Pandora.Consul/commit/1c937fc12713f8d16128f2c231748e9bee55daf9))

## [3.1.1](https://github.com/Elders/Pandora.Consul/compare/v3.1.0...v3.1.1) (2022-04-23)


### Bug Fixes

* Fixes reload bug with OptionsMonitor ([361315d](https://github.com/Elders/Pandora.Consul/commit/361315dd079ae1342b4eee0a83457694282cfb52))

# [3.1.0](https://github.com/Elders/Pandora.Consul/compare/v3.0.1...v3.1.0) (2022-03-23)


### Features

* Properly implements the consul reload functionality ([10fc295](https://github.com/Elders/Pandora.Consul/commit/10fc295a91986fe977ae29ba14f98f52b957fd0e))

## [3.0.1](https://github.com/Elders/Pandora.Consul/compare/v3.0.0...v3.0.1) (2021-12-13)


### Bug Fixes

* Adds lastIndex to request for GetAll ([e021e64](https://github.com/Elders/Pandora.Consul/commit/e021e640cc78c216888762df9692290bfcaea596))

# [3.0.0](https://github.com/Elders/Pandora.Consul/compare/v2.0.0...v3.0.0) (2021-12-13)

#### 2.0.0 - 26.03.2020
* Consul keys are case sensitive so we make everything lower
* Upgrades to netcore3.1
* Getting configurations is not bounded to the PandoraContext

#### 1.1.0 - 10.12.2018
* Updates to DNC 2.2

#### 1.0.1 - 15.11.2018
* Skips non pandora keys registered in consul

#### 1.0.0 - 15.11.2018
* Targets .netcore 2.1
* Adds PandoraConsulConfigurationSource

#### 0.8.0 - 20.06.2016
* Updates to latest Pandora. Fixed configuration merging

#### 0.7.0 - 01.06.2016
* Use consul formating for key

#### 0.6.2 - 27.11.2016
* Does not return values which are null when GetAll(...) is executed

#### 0.6.1 - 22.11.2016
* New logo

#### 0.6.0 - 15.11.2016
* Implemented GetAll method

#### 0.5.3 - 03.11.2016
* Remove the check for empty value when inserting a key/value

#### 0.5.2 - 03.11.2016
* Adds Exists(...) method for PandoraForConsul

#### 0.5.1 - 01.11.2016
* Properly builds the consul client

#### 0.5.0 - 01.11.2016
* Adds ConsulForPandora configuration repository
