<!doctype html>
<html lang="${(locale.currentLanguageTag)!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("loginTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-no-back-arrow-hover" />
    <style>
      @media (max-width: 1200px) {
        .gw-page {
          grid-template-columns: 1fr !important;
          align-content: center !important;
          overflow: visible !important;
        }

        .gw-hero {
          display: none !important;
          min-height: auto !important;
          align-items: center !important;
          justify-content: flex-start !important;
          padding: 3rem 1.25rem 0 !important;
        }

        .gw-hero .gw-brand-copy {
          width: 100% !important;
          max-width: 415px !important;
          color: #fff !important;
        }

        .gw-hero .gw-brand-copy img {
          width: 148px !important;
          margin: 0 auto 1.25rem !important;
        }

        .gw-hero .gw-kicker {
          color: rgba(147, 197, 253, 0.9) !important;
        }

        .gw-hero .gw-brand-copy h1 {
          color: #fff !important;
        }

        .gw-hero .gw-brand-copy p {
          color: rgba(255, 255, 255, 0.62) !important;
        }

        .gw-hero-footer,
        .gw-gradient-img {
          display: none !important;
        }

        .gw-auth {
          min-height: 100vh !important;
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          justify-content: center !important;
          padding: 2rem 1.25rem 2rem !important;
          gap: 1.25rem !important;
        }

        .gw-mobile-brand {
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          text-align: center !important;
          margin-bottom: 0 !important;
        }

        .gw-mobile-brand-copy h1 {
          font-size: clamp(2rem, 3.8vw, 2.9rem) !important;
        }
      }

      @media (max-width: 520px) {
        .gw-page {
          display: flex !important;
          align-items: center !important;
          justify-content: center !important;
          min-height: 100vh !important;
          overflow-x: hidden !important;
          overflow-y: auto !important;
        }

        .gw-hero {
          display: none !important;
        }

        .gw-auth {
          min-height: 100vh !important;
          min-width: 0 !important;
          width: 100% !important;
          align-items: center !important;
          justify-content: center !important;
          padding: 2rem 1rem !important;
          gap: 1.25rem !important;
        }

        .gw-mobile-brand {
          display: flex !important;
          flex-direction: column !important;
          align-items: center !important;
          text-align: center !important;
          margin-bottom: 0 !important;
        }

        .gw-mobile-brand-copy h1 {
          font-size: clamp(2rem, 3.8vw, 2.9rem) !important;
        }

        .gw-card {
          min-width: 0 !important;
          margin-left: auto !important;
          margin-right: auto !important;
        }
      }

      @media (max-width: 360px) {
        .gw-auth {
          padding: 1rem 0.75rem !important;
        }

        .gw-card {
          padding: 1.15rem !important;
        }
      }
    </style>
  </head>
  <body class="gw-body">
    <main class="gw-page">

      <div class="gw-deco" aria-hidden="true">
        <div class="gw-ring gw-ring-1"></div>
        <div class="gw-ring gw-ring-2"></div>
        <img class="gw-gradient-img" src="${url.resourcesPath}/img/gradient.png" alt="" />
      </div>

      <section class="gw-hero">

        <div class="gw-brand-copy">
          <img
            class="gw-hero-logo"
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: clamp(132px, 13vw, 190px); height: auto; max-width: 100%; margin: 0 0 1.4rem; object-fit: contain; filter: drop-shadow(0 18px 36px rgba(0, 0, 0, 0.32));"
          />
          <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
          <h1>${msg("gatewiseLoginHeroTitle")}</h1>
          <p>${msg("gatewiseLoginHeroSubtitle")}</p>
        </div>

        <footer class="gw-hero-footer">
          © 2025 GateWise. All rights reserved.
        </footer>
      </section>

      <section class="gw-auth" aria-label="${msg("loginTitle")}">
        <div class="gw-mobile-brand" aria-hidden="true">
          <img
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: 148px; height: auto; max-width: 62vw; object-fit: contain; filter: drop-shadow(0 14px 28px rgba(0, 0, 0, 0.28));"
          />
          <div class="gw-mobile-brand-copy">
            <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
            <p>${msg("gatewiseLoginHeroSubtitle")}</p>
          </div>
        </div>

        <div class="gw-card">
          <header class="gw-heading">
            <h2>${msg("gatewiseLoginTitle")}</h2>
            <p>${msg("gatewiseLoginSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>
                <#if message.type == 'error' || message.type == 'danger'>
                  ${kcSanitize(msg("invalidUserMessage"))?no_esc}
                <#else>
                  ${kcSanitize(message.summary)?no_esc}
                </#if>
              </p>
            </div>
          </#if>

          <#assign gwUsernameValue = (login.username!'')>
          <#if message?has_content && message.type == 'success'>
            <#assign gwUsernameValue = ''>
          </#if>

          <form id="kc-form-login" class="gw-form" action="${url.loginAction}" method="post" onsubmit="var b=document.getElementById('kc-login');if(b){b.disabled=true;b.classList.add('is-loading');var t=b.querySelector('.gw-submit-text');if(t){t.textContent='${msg("gatewiseSigningIn")}';}}">

            <#if usernameHidden?? && usernameHidden>
              <input type="hidden" id="username" name="username" value="${gwUsernameValue}" />
            <#else>
              <div class="gw-field">
                <label for="username">
                  <#if !realm.loginWithEmailAllowed>
                    ${msg("username")}
                  <#elseif !realm.registrationEmailAsUsername>
                    ${msg("usernameOrEmail")}
                  <#else>
                    ${msg("email")}
                  </#if>
                </label>
                <input
                  id="username"
                  name="username"
                  type="text"
                  value="${gwUsernameValue}"
                  autocomplete="username"
                  inputmode="text"
                  autofocus
                  placeholder="${msg("gatewiseUsernamePlaceholder")}"
                />
              </div>
            </#if>

            <div class="gw-field">
              <div class="gw-field-header">
                <label for="password">${msg("password")}</label>
                <#if realm.resetPasswordAllowed>
                  <a class="gw-forgot" href="${url.loginResetCredentialsUrl}">${msg("doForgotPassword")}</a>
                </#if>
              </div>
              <div class="gw-input-wrap">
                <input
                  id="password"
                  name="password"
                  type="password"
                  autocomplete="current-password"
                  placeholder="${msg("gatewisePasswordPlaceholder")}"
                />
                <button
                  class="gw-toggle"
                  type="button"
                  aria-label="${msg("gatewiseShowPassword")}"
                  onclick="var i=document.getElementById('password');i.type=i.type==='password'?'text':'password';this.classList.toggle('active');"
                >
                  <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                </button>
              </div>
            </div>

            <#if auth.selectedCredential?has_content>
              <input type="hidden" name="credentialId" value="${auth.selectedCredential}" />
            </#if>

            <div class="gw-form-footer">
              <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
                <p class="gw-register-inline">
                  ${msg("noAccount")} <a class="gw-forgot" href="${url.registrationUrl}">${msg("doRegister")}</a>
                </p>
              <#else>
                <span></span>
              </#if>
              <button class="gw-submit" name="login" id="kc-login" type="submit">
                <span class="gw-submit-spinner" aria-hidden="true"></span>
                <span class="gw-submit-text">${msg("doLogIn")}</span>
              </button>
            </div>

          </form>
        </div>
      </section>

    </main>
  </body>
</html>
