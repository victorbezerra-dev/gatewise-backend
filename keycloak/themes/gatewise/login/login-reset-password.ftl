<!doctype html>
<html lang="${(locale.currentLanguageTag)!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("gatewiseResetPasswordTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-forgot-below-remember" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/reset-password.css?v=20260607-reset-back-no-arrow-hover" />
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

      <section class="gw-auth" aria-label="${msg("gatewiseResetPasswordTitle")}">
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

        <div class="gw-card-wrapper gw-reset-wrapper">
          <a class="gw-back-button" href="${url.loginUrl}" aria-label="${msg("gatewiseBackToLogin")}">
            <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M10.8 19.2 3.6 12l7.2-7.2 1.4 1.4L7.4 11H20v2H7.4l4.8 4.8-1.4 1.4Z"/></svg>
          </a>

          <div class="gw-card gw-reset-card">
          <header class="gw-heading">
            <h2>${msg("gatewiseResetPasswordTitle")}</h2>
            <p>${msg("gatewiseResetPasswordSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>${kcSanitize(message.summary)?no_esc}</p>
            </div>
          </#if>

          <form id="kc-reset-password-form" class="gw-form" action="${url.loginAction}" method="post" onsubmit="var f=this,b=document.getElementById('kc-reset-password-submit');if(f.dataset.submitting==='true'){return true;}f.dataset.submitting='true';if(b){b.disabled=true;b.classList.add('is-loading');var t=b.querySelector('.gw-submit-text');if(t){t.textContent='${msg("gatewiseResetPasswordSending")}';}}setTimeout(function(){f.submit();},700);return false;">
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
                value="${(auth.attemptedUsername!'')}"
                autocomplete="username"
                inputmode="email"
                autofocus
                placeholder="${msg("gatewiseEmailPlaceholder")}"
              />
            </div>

            <p class="gw-reset-hint">${msg("gatewiseResetPasswordInstruction")}</p>

            <button class="gw-submit" id="kc-reset-password-submit" type="submit">
              <span class="gw-submit-spinner" aria-hidden="true"></span>
              <span class="gw-submit-text">${msg("gatewiseResetPasswordSubmit")}</span>
            </button>
          </form>
          </div>
        </div>
      </section>

    </main>
  </body>
</html>