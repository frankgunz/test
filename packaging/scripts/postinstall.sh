#!/bin/bash

msg() {
    printf "\033[32m $1\033[0m\n"
}

msg "Running post-install actions..."

if id "ehubbadmin" &>/dev/null; then
    msg "ehubbadmin found, skipping user creation..."
else
    msg "Creating ehubbadmin user..."
    adduser ehubbadmin --disabled-password --gecos "EnterpriseHubb Admin"

    msg "Setting ehubbadmin user with a default password..."
    echo "ehubbadmin:hubbell" | chpasswd

    if getent group sudo &>/dev/null; then
        msg "Adding sudo group to ehubbadmin user..."
        usermod -aG sudo ehubbadmin
    fi
fi

msg "Adding logging directory and symlink..."

mkdir -vp /var/log/EnterpriseHubb

ln -sv /var/log/EnterpriseHubb /lib/EnterpriseHubb/logs 

msg "Changing permissions of directories and files..."

chown -R ehubbadmin:ehubbadmin /lib/EnterpriseHubb

chown -R ehubbadmin:ehubbadmin /lib/EnterpriseHubb/*

chown -R ehubbadmin:ehubbadmin /var/log/EnterpriseHubb

chown -R ehubbadmin:ehubbadmin /usr/share/EnterpriseHubb/*

if [ "$IS_CONTAINER" == "true" ]; then
  exit 0
fi

msg "Making sure all services are enabled by default..."

systemctl daemon-reload

for s in /lib/systemd/system/ehubb-*.service; do systemctl enable "$s"; done 