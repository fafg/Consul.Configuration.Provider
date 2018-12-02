#!/bin/bash

set -e

# Environment check
 [ -z "$coverityToken" ] && echo "Need to set a coverity token" && exit 1

SOURCE_DIR="../"

case $(uname -m) in
	i?86)				BITS=32 ;;
	amd64|x86_64)	BITS=64 ;;
esac
SCAN_TOOL=https://scan.coverity.com/download/linux${BITS}
TOOL_BASE=$(pwd)/ci/_coverity-scan

# Install coverity tools
if [ ! -d "$TOOL_BASE" ]; then
	echo "Downloading coverity..."
	mkdir -p "$TOOL_BASE"
	pushd "$TOOL_BASE"
	wget -O coverity_tool.tgz $SCAN_TOOL \
		--post-data "token=$coverityToken&project=Consul.Configuration.Provider"
	tar xzf coverity_tool.tgz
	popd
	TOOL_DIR=$(find "$TOOL_BASE" -type d -name 'cov-analysis*')
	ln -s "$TOOL_DIR" "$TOOL_BASE"/cov-analysis
fi

COV_BUILD="$TOOL_BASE/cov-analysis/bin/cov-build"

COVERITY_UNSUPPORTED=1 \
	$COV_BUILD --dir cov-int \
	dotnet build -c Release ../Consul.Configuration.Provider.sln

# Upload results
tar czf Consul.Configuration.Provider.tgz cov-int
SHA=$(cd ${SOURCE_DIR} && git rev-parse --short HEAD)

HTML="$(curl \
	--silent \
	--write-out "\n%{http_code}" \
	--form token="$COVERITY_TOKEN" \
	--form email=Consul.Configuration.Provider@gmail.com \
	--form file=@Consul.Configuration.Provider.tgz \
	--form version="$SHA" \
	--form description="Consul.Configuration.Provider build" \
	https://scan.coverity.com/builds?project=Consul.Configuration.Provider)"
# Body is everything up to the last line
BODY="$(echo "$HTML" | head -n-1)"
# Status code is the last line
STATUS_CODE="$(echo "$HTML" | tail -n1)"

echo "${BODY}"

if [ "${STATUS_CODE}" != "200" -a "${STATUS_CODE}" != "201" ]; then
	echo "Received error code ${STATUS_CODE} from Coverity"
	exit 1
fi
