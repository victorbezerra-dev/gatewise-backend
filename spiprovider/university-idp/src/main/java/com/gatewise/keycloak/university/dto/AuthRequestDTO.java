package com.gatewise.keycloak.university.dto;

public class AuthRequestDTO {
    private String usuario;
    private String senha;
    private String tokenFirebase;
    private int codigoVersao;
    private String nomeVersao;
    private String nomeSistemaOperacional;
    private String versaoSistemaOperacional;
    private String modeloDispositivo;
    private String nomeFabricanteDispositivo;

    public AuthRequestDTO() {}

    public AuthRequestDTO(String usuario, String senha) {
        this.usuario = usuario;
        this.senha = senha;
        this.tokenFirebase = "";
        this.codigoVersao = 122;
        this.nomeVersao = "1.7.3";
        this.nomeSistemaOperacional = "Android";
        this.versaoSistemaOperacional = "14";
        this.modeloDispositivo = "SM-A556E";
        this.nomeFabricanteDispositivo = "Samsung";
    }

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(String usuario) {
        this.usuario = usuario;
    }

    public String getSenha() {
        return senha;
    }

    public void setSenha(String senha) {
        this.senha = senha;
    }

    public String getTokenFirebase() {
        return tokenFirebase;
    }

    public void setTokenFirebase(String tokenFirebase) {
        this.tokenFirebase = tokenFirebase;
    }

    public int getCodigoVersao() {
        return codigoVersao;
    }

    public void setCodigoVersao(int codigoVersao) {
        this.codigoVersao = codigoVersao;
    }

    public String getNomeVersao() {
        return nomeVersao;
    }

    public void setNomeVersao(String nomeVersao) {
        this.nomeVersao = nomeVersao;
    }

    public String getNomeSistemaOperacional() {
        return nomeSistemaOperacional;
    }

    public void setNomeSistemaOperacional(String nomeSistemaOperacional) {
        this.nomeSistemaOperacional = nomeSistemaOperacional;
    }

    public String getVersaoSistemaOperacional() {
        return versaoSistemaOperacional;
    }

    public void setVersaoSistemaOperacional(String versaoSistemaOperacional) {
        this.versaoSistemaOperacional = versaoSistemaOperacional;
    }

    public String getModeloDispositivo() {
        return modeloDispositivo;
    }

    public void setModeloDispositivo(String modeloDispositivo) {
        this.modeloDispositivo = modeloDispositivo;
    }

    public String getNomeFabricanteDispositivo() {
        return nomeFabricanteDispositivo;
    }

    public void setNomeFabricanteDispositivo(String nomeFabricanteDispositivo) {
        this.nomeFabricanteDispositivo = nomeFabricanteDispositivo;
    }
}
