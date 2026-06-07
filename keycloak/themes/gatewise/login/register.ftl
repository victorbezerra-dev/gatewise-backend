<!doctype html>
<html lang="${(locale.currentLanguageTag)!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("gatewiseRegisterTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-no-back-arrow-hover" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/register.css?v=20260607-register-back-no-arrow-hover" />
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

      <section class="gw-auth" aria-label="${msg("gatewiseRegisterTitle")}">
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

        <div class="gw-card-wrapper gw-register-wrapper">
          <a class="gw-back-button" href="${url.loginUrl}" aria-label="${msg("gatewiseBackToLogin")}">
            <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M10.8 19.2 3.6 12l7.2-7.2 1.4 1.4L7.4 11H20v2H7.4l4.8 4.8-1.4 1.4Z"/></svg>
            <span>${msg("gatewiseBackToLogin")}</span>
          </a>

          <div class="gw-card gw-register-card">
          <header class="gw-heading">
            <h2>${msg("gatewiseRegisterTitle")}</h2>
            <p>${msg("gatewiseRegisterSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>${kcSanitize(message.summary)?no_esc}</p>
            </div>
          </#if>

          <form id="kc-register-form" class="gw-form" action="${url.registrationAction}" method="post">
            <div class="gw-register-grid">
              <div class="gw-field">
                <label for="firstName">${msg("firstName")}</label>
                <input id="firstName" name="firstName" type="text" value="${((register.formData.firstName)!'')}" autocomplete="given-name" placeholder="${msg("gatewiseFirstNamePlaceholder")}" />
              </div>

              <div class="gw-field">
                <label for="lastName">${msg("lastName")}</label>
                <input id="lastName" name="lastName" type="text" value="${((register.formData.lastName)!'')}" autocomplete="family-name" placeholder="${msg("gatewiseLastNamePlaceholder")}" />
              </div>
            </div>

            <div class="gw-field">
              <label for="email">${msg("email")}</label>
              <input id="email" name="email" type="email" value="${((register.formData.email)!'')}" autocomplete="email" placeholder="${msg("gatewiseEmailPlaceholder")}" />
            </div>

            <#if !realm.registrationEmailAsUsername>
              <div class="gw-field">
                <label for="username">${msg("username")}</label>
                <input id="username" name="username" type="text" value="${((register.formData.username)!'')}" autocomplete="username" placeholder="${msg("gatewiseUsernamePlaceholder")}" />
              </div>
            </#if>

            <#if passwordRequired??>
              <div class="gw-field">
                <label for="password">${msg("password")}</label>
                <div class="gw-input-wrap">
                  <input id="password" name="password" type="password" autocomplete="new-password" placeholder="${msg("gatewisePasswordPlaceholder")}" />
                  <button class="gw-toggle" type="button" aria-label="${msg("gatewiseShowPassword")}" onclick="var i=document.getElementById('password');i.type=i.type==='password'?'text':'password';this.classList.toggle('active');">
                    <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                  </button>
                </div>
              </div>

              <div class="gw-field">
                <label for="password-confirm">${msg("passwordConfirm")}</label>
                <div class="gw-input-wrap">
                  <input id="password-confirm" name="password-confirm" type="password" autocomplete="new-password" placeholder="${msg("gatewisePasswordConfirmPlaceholder")}" />
                  <button class="gw-toggle" type="button" aria-label="${msg("gatewiseShowPassword")}" onclick="var i=document.getElementById('password-confirm');i.type=i.type==='password'?'text':'password';this.classList.toggle('active');">
                    <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                  </button>
                </div>
              </div>
            </#if>

            <#if recaptchaRequired??>
              <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
            </#if>

            <div class="gw-form-footer gw-register-footer">
              <p class="gw-register-inline">
                ${msg("alreadyAccount")} <a href="${url.loginUrl}">${msg("doLogIn")}</a>
              </p>
              <button class="gw-submit" type="submit">
                <span class="gw-submit-text">${msg("doRegister")}</span>
              </button>
            </div>
          </form>
          </div>
        </div>
      </section>
    </main>
  </body>
</html>