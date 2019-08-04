# Lines configured by zsh-newuser-install
HISTFILE=~/.histfile
HISTSIZE=1000
SAVEHIST=1000
setopt appendhistory autocd beep extendedglob nomatch notify promptsubst
bindkey -v
# End of lines configured by zsh-newuser-install
# The following lines were added by compinstall
zstyle :compinstall filename "/home/${USER}/.zshrc"

autoload -Uz compinit
compinit
# End of lines added by compinstall

# Configure a default text editor
export EDITOR=nvim

# Add dotnet global tools to the path
export PATH="${PATH}:/root/.dotnet/tools"

# Start of antigen configuration
source /usr/share/zsh-antigen/antigen.zsh

# Configuration for plguins
# uncomment next line to have ssh-agent automatically load keys
zstyle :omz:plugins:ssh-agent identities id_ed25519 id_github id_gitlab

# Load the oh-my-zsh's library.
antigen user oh-my-zsh

# Bundles from the default repo (robbyrussell's oh-my-zsh).
antigen bundle command-not-found
antigen bundle common-aliases
antigen bundle git
antigen bundle ng
antigen bundle npm
antigen bundle ssh-agent
antigen bundle sudo

# Syntax highlighting bundle.
antigen bundle zsh-users/zsh-syntax-highlighting

# Load the theme.
antigen theme https://github.com/agnoster/agnoster-zsh-theme agnoster

# Tell Antigen that you're done.
antigen apply
